using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using SOSXR.SeaShark.EditorScripts;
using UnityEditor;
using UnityEngine;

namespace SOSXR.SeaShark.Tests
{
    /// <summary>
    ///     Covers the 0.7.2 fixes to the attributes in Runtime/GeneralAttributes and their drawers in Editor/General.
    /// </summary>
    [TestFixture]
    public class AttributeDrawerTests
    {
        private readonly List<Object> _created = new();

        [TearDown]
        public void TearDown()
        {
            foreach (var obj in _created)
            {
                if (obj != null)
                {
                    Object.DestroyImmediate(obj);
                }
            }

            _created.Clear();
        }

        private T Track<T>(T obj)
            where T : Object
        {
            _created.Add(obj);

            return obj;
        }

        private T CreateTarget<T>()
            where T : ScriptableObject
        {
            return Track(ScriptableObject.CreateInstance<T>());
        }

        // --- Attributes that must apply to a List or array as a whole ---------------------------------------------

        private static IEnumerable<TestCaseData> CollectionAttributes()
        {
            yield return new TestCaseData(new InfoAttribute("x")).SetName("ApplyToCollection_Info");
            yield return new TestCaseData(new HorizontalLineAttribute()).SetName("ApplyToCollection_HorizontalLine");
            yield return new TestCaseData(new ReadOnlyGroupBeginAttribute()).SetName("ApplyToCollection_ReadOnlyGroupBegin");
            yield return new TestCaseData(new ReadOnlyGroupEndAttribute()).SetName("ApplyToCollection_ReadOnlyGroupEnd");
            yield return new TestCaseData(new ReadOnlyAttribute()).SetName("ApplyToCollection_ReadOnly");
            yield return new TestCaseData(new DisableEditingAttribute()).SetName("ApplyToCollection_DisableEditing");
            yield return new TestCaseData(new ShowIfAttribute("x")).SetName("ApplyToCollection_ShowIf");
            yield return new TestCaseData(new EnableIfAttribute("x")).SetName("ApplyToCollection_EnableIf");
            yield return new TestCaseData(new HideIfAttribute("x", true)).SetName("ApplyToCollection_HideIf");
            yield return new TestCaseData(new HideIfNullAttribute("x")).SetName("ApplyToCollection_HideIfNull");
            yield return new TestCaseData(new HideIfNotNullAttribute("x")).SetName("ApplyToCollection_HideIfNotNull");
            yield return new TestCaseData(new HideIfEnumValueAttribute("x", HideIf.Equal, 1)).SetName("ApplyToCollection_HideIfEnumValue");
            yield return new TestCaseData(new RequiredAttribute()).SetName("ApplyToCollection_Required");
            yield return new TestCaseData(new LabelSearchAttribute("x")).SetName("ApplyToCollection_LabelSearch");
            yield return new TestCaseData(new LabelSearchAttribute("x", 3)).SetName("ApplyToCollection_LabelSearch_Limit");
            yield return new TestCaseData(new LabelSearchAttribute("x", LabelSearchAttribute.Direction.DESC)).SetName("ApplyToCollection_LabelSearch_Direction");
            yield return new TestCaseData(new LabelSearchAttribute("x", 3, LabelSearchAttribute.Direction.DESC)).SetName("ApplyToCollection_LabelSearch_LimitDirection");
        }

        [TestCaseSource(nameof(CollectionAttributes))]
        public void Attribute_AppliesToCollection(UnityEngine.PropertyAttribute attribute)
        {
            Assert.IsTrue(attribute.applyToCollection);
        }

        [Test]
        public void PerElementAttribute_StillAppliesToElements()
        {
            // Preview draws one object reference, so on a List it must keep applying to each element.
            Assert.IsFalse(new PreviewAttribute().applyToCollection);
        }

        // --- Required ------------------------------------------------------------------------------------------

        [Test]
        public void Required_EmptyList_IsEmpty_AndFilledList_IsNot()
        {
            var so = new SerializedObject(CreateTarget<RequiredTarget>());
            var items = so.FindProperty(nameof(RequiredTarget.Items));

            Assert.IsTrue(RequiredAttributeDrawer.IsFieldSupported(items));
            Assert.IsTrue(RequiredAttributeDrawer.IsFieldEmpty(items));

            items.arraySize = 1;

            Assert.IsFalse(RequiredAttributeDrawer.IsFieldEmpty(items));
        }

        [Test]
        public void Required_EmptyArray_IsEmpty()
        {
            var so = new SerializedObject(CreateTarget<RequiredTarget>());
            var array = so.FindProperty(nameof(RequiredTarget.Array));

            Assert.IsTrue(RequiredAttributeDrawer.IsFieldSupported(array));
            Assert.IsTrue(RequiredAttributeDrawer.IsFieldEmpty(array));
        }

        [Test]
        public void Required_String_IsCheckedByText_NotAsCollection()
        {
            var so = new SerializedObject(CreateTarget<RequiredTarget>());
            var text = so.FindProperty(nameof(RequiredTarget.Text));

            Assert.IsTrue(RequiredAttributeDrawer.IsFieldEmpty(text));

            text.stringValue = "abc";

            Assert.IsFalse(RequiredAttributeDrawer.IsFieldEmpty(text));
        }

        [Test]
        public void Required_NullReference_And_ClearColor_AreEmpty()
        {
            var so = new SerializedObject(CreateTarget<RequiredTarget>());

            Assert.IsTrue(RequiredAttributeDrawer.IsFieldEmpty(so.FindProperty(nameof(RequiredTarget.Reference))));
            Assert.IsTrue(RequiredAttributeDrawer.IsFieldEmpty(so.FindProperty(nameof(RequiredTarget.Tint))));
        }

        [Test]
        public void Required_Int_IsNotSupported()
        {
            var so = new SerializedObject(CreateTarget<RequiredTarget>());

            Assert.IsFalse(RequiredAttributeDrawer.IsFieldSupported(so.FindProperty(nameof(RequiredTarget.Number))));
        }

        // --- ShowIf / SerializedPropertyUtility ----------------------------------------------------------------

        [TestCase(false)]
        [TestCase(true)]
        public void ShowIf_TopLevelCondition(bool flag)
        {
            var target = CreateTarget<ShowIfTarget>();
            target.Flag = flag;
            var so = new SerializedObject(target);

            Assert.AreEqual(flag, ShowIfDrawer.ShouldShow(so.FindProperty(nameof(ShowIfTarget.Value)), nameof(ShowIfTarget.Flag)));
        }

        [TestCase(false)]
        [TestCase(true)]
        public void ShowIf_PrivateConditionOnBaseClass(bool flag)
        {
            var target = CreateTarget<ShowIfTarget>();
            target.SetBaseFlag(flag);
            var so = new SerializedObject(target);

            Assert.AreEqual(flag, ShowIfDrawer.ShouldShow(so.FindProperty(nameof(ShowIfTarget.Value)), "_baseFlag"));
        }

        [TestCase(false)]
        [TestCase(true)]
        public void ShowIf_ConditionInsideNestedClass(bool show)
        {
            var target = CreateTarget<ShowIfTarget>();
            target.Nested.Show = show;
            var so = new SerializedObject(target);
            var property = so.FindProperty($"{nameof(ShowIfTarget.Nested)}.{nameof(ShowIfNested.Value)}");

            Assert.AreEqual(show, ShowIfDrawer.ShouldShow(property, nameof(ShowIfNested.Show)));
        }

        [TestCase(0, false)]
        [TestCase(1, true)]
        public void ShowIf_ConditionInsideListElement(int index, bool expected)
        {
            var so = new SerializedObject(CreateTarget<ShowIfTarget>());
            var property = so.FindProperty($"{nameof(ShowIfTarget.Items)}.Array.data[{index}].{nameof(ShowIfNested.Value)}");

            Assert.AreEqual(expected, ShowIfDrawer.ShouldShow(property, nameof(ShowIfNested.Show)));
        }

        [Test]
        public void ShowIf_MissingCondition_Shows()
        {
            var so = new SerializedObject(CreateTarget<ShowIfTarget>());

            Assert.IsTrue(ShowIfDrawer.ShouldShow(so.FindProperty(nameof(ShowIfTarget.Value)), "DoesNotExist"));
        }

        [Test]
        public void GetParentObject_NestedField_ReturnsNestedInstance()
        {
            var target = CreateTarget<ShowIfTarget>();
            var so = new SerializedObject(target);
            var property = so.FindProperty($"{nameof(ShowIfTarget.Nested)}.{nameof(ShowIfNested.Value)}");

            Assert.AreSame(target.Nested, SerializedPropertyUtility.GetParentObject(property));
        }

        [Test]
        public void GetParentObject_TopLevelField_ReturnsTarget()
        {
            var target = CreateTarget<ShowIfTarget>();
            var so = new SerializedObject(target);

            Assert.AreSame(target, SerializedPropertyUtility.GetParentObject(so.FindProperty(nameof(ShowIfTarget.Items))));
        }

        [Test]
        public void FindField_FindsPrivateFieldOnBaseClass()
        {
            Assert.IsNotNull(SerializedPropertyUtility.FindField(typeof(ShowIfTarget), "_baseFlag"));
            Assert.IsNull(SerializedPropertyUtility.FindField(typeof(ShowIfTarget), "DoesNotExist"));
        }

        // --- Preview -------------------------------------------------------------------------------------------

        [Test]
        public void Preview_Null_ReturnsNull()
        {
            Assert.IsNull(PreviewDrawer.GetPreviewTexture(null));
        }

        [Test]
        public void Preview_Texture_ReturnsTexture()
        {
            var texture = Track(new Texture2D(2, 2));

            Assert.AreSame(texture, PreviewDrawer.GetPreviewTexture(texture));
        }

        [Test]
        public void Preview_Sprite_ReturnsItsTexture()
        {
            var texture = Track(new Texture2D(2, 2));
            var sprite = Track(Sprite.Create(texture, new Rect(0, 0, 2, 2), Vector2.zero));

            Assert.AreSame(texture, PreviewDrawer.GetPreviewTexture(sprite));
        }

        [Test]
        public void Preview_GameObjectWithoutRenderer_ReturnsNull()
        {
            var go = Track(new GameObject("PreviewNoRenderer"));

            Assert.IsNull(PreviewDrawer.GetPreviewTexture(go));
        }

        [Test]
        public void Preview_RendererWithoutMaterial_ReturnsNull_WithoutThrowing()
        {
            var go = Track(new GameObject("PreviewNoMaterial"));
            go.AddComponent<MeshRenderer>();

            Assert.DoesNotThrow(() => PreviewDrawer.GetPreviewTexture(go));
            Assert.IsNull(PreviewDrawer.GetPreviewTexture(go));
        }

        [Test]
        public void Preview_RendererWithMaterial_ReturnsMainTexture()
        {
            var shader = Shader.Find("Unlit/Texture");
            Assume.That(shader, Is.Not.Null, "Built-in 'Unlit/Texture' shader not available.");

            var texture = Track(new Texture2D(2, 2));
            var material = Track(new Material(shader) { mainTexture = texture });
            var go = Track(new GameObject("PreviewWithMaterial"));
            go.AddComponent<MeshRenderer>().sharedMaterial = material;

            Assert.AreSame(texture, PreviewDrawer.GetPreviewTexture(go));
        }

        // --- Info on fields Unity does not serialize -------------------------------------------------------------

        [Test]
        public void InfoCollector_ReturnsOnlyUnserializedFields()
        {
            var so = new SerializedObject(CreateTarget<InfoTarget>());
            var texts = InfoFieldCollector.GetUnserializedFieldInfos(so).Select(i => i.Info.InfoText).ToList();

            CollectionAssert.AreEquivalent(new[] { "base private", "private", "static", "one", "two" }, texts);
        }

        [Test]
        public void InfoCollector_ListsBaseClassFieldsFirst()
        {
            var so = new SerializedObject(CreateTarget<InfoTarget>());
            var texts = InfoFieldCollector.GetUnserializedFieldInfos(so).Select(i => i.Info.InfoText).ToList();

            Assert.AreEqual("base private", texts[0]);
        }

        [Test]
        public void InfoCollector_LabelIsNicifiedFieldName()
        {
            var so = new SerializedObject(CreateTarget<InfoTarget>());
            var infos = InfoFieldCollector.GetUnserializedFieldInfos(so);

            Assert.AreEqual(ObjectNames.NicifyVariableName("_private"), infos.Single(i => i.Info.InfoText == "private").Label);
        }

        [Test]
        public void InfoCollector_NullSerializedObject_ReturnsEmpty()
        {
            Assert.IsEmpty(InfoFieldCollector.GetUnserializedFieldInfos(null));
        }
    }
}

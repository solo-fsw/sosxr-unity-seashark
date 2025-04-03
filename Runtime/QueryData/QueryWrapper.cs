using SOSXR.SeaShark.Attributes;
using UnityEngine;


namespace SOSXR.SeaShark.QueryData
{
    public class QueryWrapper : MonoBehaviour
    {
        [SerializeReference] [Interface(typeof(IHaveQueryData))] public Object DataObject;
        public IHaveQueryData QueryData => DataObject as IHaveQueryData;


        [ContextMenu(nameof(UpdateQuery))]
        public void UpdateQuery()
        {
            QueryData?.UpdateQuery();
        }


        [ContextMenu(nameof(ClearQueryData))]
        public void ClearQueryData()
        {
            QueryData?.ClearQuery();
        }
    }
}
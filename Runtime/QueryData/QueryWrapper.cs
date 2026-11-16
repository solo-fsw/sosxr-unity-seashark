using UnityEngine;


namespace SOSXR.SeaShark
{
    /// <summary>
    ///     Forwards query actions to the IHaveQueryData object assigned in DataObject.
    /// </summary>
    public class QueryWrapper : MonoBehaviour
    {
        /// <summary>
        ///     Object that implements IHaveQueryData, assigned in the Inspector.
        /// </summary>
        [SerializeField] [Interface(typeof(IHaveQueryData))]
        public Object DataObject;

        /// <summary>
        ///     The DataObject as IHaveQueryData, or null if it does not implement the interface.
        /// </summary>
        public IHaveQueryData QueryData => DataObject as IHaveQueryData;


        /// <summary>
        ///     Calls UpdateQuery on the query data, if any. Available from the component context menu.
        /// </summary>
        [ContextMenu(nameof(UpdateQuery))]
        public void UpdateQuery()
        {
            QueryData?.UpdateQuery();
        }


        /// <summary>
        ///     Calls ClearQuery on the query data, if any. Available from the component context menu.
        /// </summary>
        [ContextMenu(nameof(ClearQueryData))]
        public void ClearQueryData()
        {
            QueryData?.ClearQuery();
        }
    }
}
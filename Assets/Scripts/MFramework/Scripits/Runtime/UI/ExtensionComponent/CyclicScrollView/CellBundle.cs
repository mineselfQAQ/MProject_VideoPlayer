using UnityEngine;

namespace MFramework
{
    /// <summary>
    /// Cell���ϣ����д����һ��/һ��Cell
    /// </summary>
    public class CellBundle<Cell> : IPoolObject where Cell : MonoBehaviour
    {
        internal int index;
        internal Vector2 position;

        internal Cell[] Cells { get; private set; }
        internal int CellCapacity => Cells.Length;

        internal CellBundle(int capacity)

        {
            Cells = new Cell[capacity];
        }

        /// <summary>
        /// ��Bundle�е�ȫ��Ԫ�ؽ���
        /// </summary>
        public void Clear()
        {
            index = -1;
            foreach (var cell in Cells)
            {
                if (cell != null)
                {
                    cell.gameObject.SetActive(false);
                }
            }
        }
    }
}
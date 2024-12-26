using System;
using System.Collections;

namespace MFramework.DLC
{
    public class MAVLTreeNode
    {
        #region Internal Field
        internal MAVLTree list;

        internal object item;

        internal MAVLTreeNode parent;

        internal MAVLTreeNode left;

        internal MAVLTreeNode right;

        internal int height;
        #endregion

        #region Public Field
        public MAVLTree List => list;

        public object Value
        {
            get
            {
                return item;
            }
            set
            {
                item = value;
            }
        }

        public MAVLTreeNode Parent => parent;

        public MAVLTreeNode Left => left;

        public MAVLTreeNode Right => right;
        #endregion

        #region Constructor
        public MAVLTreeNode(object value)
        {
            item = value;
        }

        internal MAVLTreeNode(object value, MAVLTree list)
        {
            this.list = list;
            item = value;
        }
        #endregion

        #region Function
        //ʹ�ڵ���Ч(û���������������ϵ����GC����)
        internal void Invalidate()
        {
            list = null;//����---list��ս�ͨ����ValidateNode()���Ӷ��޷������κβ���
            parent = null;
            left = null;
            right = null;
        }
        #endregion
    }

    public class MAVLTree : IEnumerable
    {
        #region Internal Field
        private MAVLTreeNode root;

        private int count;
        #endregion

        #region Public Field
        public MAVLTreeNode Root => root;

        public int Count => count;
        #endregion

        #region Constructor
        public MAVLTree() { }
        #endregion

        #region Function
        public void Add(object o)
        {
            root = InternalAdd(root, o, null);
            count++;
        }

        public void Remove(object o)
        {
            if (root == null) throw new Exception();//rootΪ����ô��������������Ҫremove

            InternalRemove(root, o);
            count--;
        }

        public bool Contains(object o)
        {
            return Search(o) != null;
        }

        public object[] Sort()
        {
            return InOrder();
        }

        private MAVLTreeNode InternalAdd(MAVLTreeNode node, object o, MAVLTreeNode beforeNode)
        {
            if (node == null)
            {
                MAVLTreeNode newNode = new MAVLTreeNode(o, this);
                newNode.parent = beforeNode;
                return newNode;
            }

            IComparable num = o as IComparable;
            if (num == null) throw new Exception();//����û��IComparable�����ݲ��ܱȽ�
            
            if (num.CompareTo(node.item) < 0)//����ֵ��С��������
            {
                node.left = InternalAdd(node.left, o, node);
            }
            else if (num.CompareTo(node.item) > 0)//����ֵ����������
            {
                node.right = InternalAdd(node.right, o, node);
            }
            else
            {
                //����������ز��������
                throw new Exception();
            }

            UpdateHeight(node);
            node = Rotate(node);

            return node;
        }
        private MAVLTreeNode InternalRemove(MAVLTreeNode node, object o)
        {
            if (node == null) return null;

            IComparable num = o as IComparable;
            if (num == null) throw new Exception();//����û��IComparable�����ݲ��ܱȽ�

            if (num.CompareTo(node.item) < 0)//����ֵ��С��������
            {
                node.left = InternalRemove(node.left, o);
            }
            else if (num.CompareTo(node.item) > 0)//����ֵ����������
            {
                node.right = InternalRemove(node.right, o);
            }
            else
            {
                //��Ϊ0(ΪҶ�ӽڵ�)
                if (node.left == null && node.right == null)
                {
                    //Ҷ�ӽڵ��null�᷵�ظ���һ�㣬��ô��һ�����һ�������Ϊnull������InvalidNode()
                    node.Invalidate();
                    return null;
                }
                //��Ϊ1
                else if (node.left == null || node.right == null)
                {
                    MAVLTreeNode childNode = node.left ?? node.right;

                    //��һ��д��
                    //node = childNode;
                    //ע�⣺��Ҫ���ڵ�Invalidate()������ͨ��temp�洢node�ڸ�ֵ��ɾ��
                    //����Ϊ����nodeָ��childNode�Ķ��ڴ棬��ôchildNode�ͱ�"��������"��
                    //����node�����Ǹ�node��node.parent��node�Ĺ�ϵû�з����ı�

                    if (node == root)
                    {
                        root = childNode;
                    }
                    else
                    {
                        if (node.parent.left == node)//�ýڵ�Ϊ��ڵ�
                        {
                            node.parent.left = childNode;
                        }
                        else if (node.parent.right == node)//�ýڵ�Ϊ�ҽڵ�
                        {
                            node.parent.right = childNode;
                        }
                    }
                    node.Invalidate();//���ڵ��Ѿ����������ڵ㣬����Ҫʹ��InvalidNode()(���ڸ��ڵ�Ҳ������)
                }
                //��Ϊ2
                else
                {
                    MAVLTreeNode tempNode = node.right;
                    while (tempNode.left != null)
                    {
                        tempNode = tempNode.left;
                    }
                    node.item = tempNode.item;
                    InvalidNode(tempNode);
                }
            }

            UpdateHeight(node);
            node = Rotate(node);

            return node;
        }

        private MAVLTreeNode Search(object o)
        {
            IComparable num = o as IComparable;
            if (num == null) throw new Exception();//����û��IComparable�����ݲ��ܱȽ�

            MAVLTreeNode cur = root;

            while (cur != null)
            {
                if (num.CompareTo(cur.item) > 0)//num > cur.item
                {
                    cur = cur.right;//����ֵ����������
                }
                else if (num.CompareTo(cur.item) < 0)//num < cur.item
                {
                    cur = cur.left;//����ֵ��С��������
                }
                else
                {
                    break;//���Ǹ�ֵ����ȡ
                }
            }
            return cur;
        }

        private void ValidateNode(MAVLTreeNode node)
        {
            //�ڵ�������
            if (node == null)
            {
                throw new Exception();
            }
            //�ڵ���������Ӧ��Ϊͬһ��
            if (node.list != this)
            {
                throw new Exception();
            }
        }
        private void ValidateNewNode(MAVLTreeNode node)
        {
            //�ڵ�������
            if (node == null)
            {
                throw new Exception();
            }
            //�½ڵ��listӦ��Ϊnull(���ⲿ������node��list��null��)
            if (node.list != null)
            {
                throw new Exception();
            }
        }
        private void InvalidNode(MAVLTreeNode node)
        {
            //�����ڵ��⣬һ��ڵ㶼��Ҫ�����ڵ���ýڵ�Ͽ�����
            if (node != root)
            {
                MAVLTreeNode parentNode = node.parent;

                //���ݽڵ��ڸ��ڵ�����ҶϿ�
                if (parentNode.left == node)
                {
                    parentNode.left = null;
                }
                else if (parentNode.right == node)
                {
                    parentNode.right = null;
                }
            }

            //�Ͽ��Լ�������
            node.Invalidate();
        }

        private int GetHeight(MAVLTreeNode node) => node == null ? -1 : node.height;
        private void UpdateHeight(MAVLTreeNode node)
        {
            node.height = Math.Max(GetHeight(node.left), GetHeight(node.right)) + 1;
        }
        private int BalanceFactor(MAVLTreeNode node)
        {
            if (node == null) return 0;

            return GetHeight(node.left) - GetHeight(node.right);
        }

        //Tip:������ɺ�δ�븸�ڵ�����
        private MAVLTreeNode RightRotate(MAVLTreeNode node)
        {
            MAVLTreeNode childNode = node.left;

            //�����ϲ��ᷢ��������������ӽڵ㶼û�ҵ�����ô�Ͳ���������
            if (childNode == null)
            {
                return node;
            }

            if (childNode.right == null)//childNode���ӽڵ����
            {
                childNode.parent = node.parent;
                childNode.right = node;
                node.left = null;
                node.parent = childNode;
            }
            else//childNode���ӽڵ����
            {
                MAVLTreeNode grandChildNode = childNode.right;
                childNode.parent = node.parent;
                childNode.right = node;
                node.left = grandChildNode;
                node.parent = childNode;
                grandChildNode.parent = node;
            }

            //node��childNode˳�����ı䣬�߶���Ȼ�����ı䣬���ӽڵ㲻��Ӱ��
            UpdateHeight(node);
            UpdateHeight(childNode);

            return childNode;//Ϊ���º��"���ڵ�"
        }
        private MAVLTreeNode LeftRotate(MAVLTreeNode node)
        {
            MAVLTreeNode childNode = node.right;

            if (childNode == null)
            {
                return node;
            }

            if (childNode.left == null)//childNode���ӽڵ����
            {
                childNode.parent = node.parent;
                childNode.left = node;
                node.right = null;
                node.parent = childNode;
            }
            else//childNode���ӽڵ����
            {
                MAVLTreeNode grandChildNode = childNode.left;
                childNode.parent = node.parent;
                childNode.left = node;
                node.right = grandChildNode;
                node.parent = childNode;
                grandChildNode.parent = node;
            }

            //node��childNode˳�����ı䣬�߶���Ȼ�����ı䣬���ӽڵ㲻��Ӱ��
            UpdateHeight(node);
            UpdateHeight(childNode);

            return childNode;//Ϊ���º��"���ڵ�"
        }
        private MAVLTreeNode Rotate(MAVLTreeNode node)
        {
            int balanceFactor = BalanceFactor(node);

            if (balanceFactor > 1)//��ƫ�����
            {
                if (BalanceFactor(node.left) >= 0)//�ڵ������
                {
                    //����
                    return RightRotate(node);
                }
                else//�ڵ����Ҳ�
                {
                    //��������������
                    node.left = LeftRotate(node.left);
                    return RightRotate(node);
                }
            }
            if (balanceFactor < -1)//��ƫ�����
            {
                if (BalanceFactor(node.right) <= 0)//�ڵ����Ҳ�
                {
                    //����
                    return LeftRotate(node);
                }
                else//�ڵ������
                {
                    //��������������
                    node.right = RightRotate(node.right);
                    return LeftRotate(node);
                }
            }
            //û��ʧ�⣬ֱ�ӷ���
            return node;
        }

        public IEnumerator GetEnumerator()
        {
            foreach (var item in InOrder())
            {
                yield return item;
            }
        }
        private object[] InOrder()
        {
            ValidateNode(root);

            object[] list = new object[count];
            InternalInOrder(root, list, 0);

            return list;
        }
        private int InternalInOrder(MAVLTreeNode root, object[] list, int index)
        {
            if (root == null) return index - 1;
            int next = InternalInOrder(root.left, list, index);
            next++;
            list[next] = root.item;
            return InternalInOrder(root.right, list, next + 1);
        }

        #endregion
    }

    public static class MAVLTreeExtension
    {
        public static void Print(this MAVLTree tree)
        {
            MQueue queue = new MQueue();
            queue.Enqueue(tree.Root);

            int count = 0;
            int beforeCount = 0;
            int level = 0;
            string levelStr = "";
            bool isNotBottom = true;
            while (queue.Count != 0)
            {
                MAVLTreeNode node = (MAVLTreeNode)queue.Dequeue();
                count++;

                //��������������ͻ���
                if (count == 1 || count == beforeCount * 2)
                {
                    //���û����һ��(Ϊ�ײ�����һ��)���ͽ���
                    if (!isNotBottom) break;
                    isNotBottom = false;

                    beforeCount = count;
                    level++;

                    levelStr += $"\n��{level}: ";
                }
                //���
                if (node != null)
                {
                    levelStr += $"{node.Value} ";
                }
                else
                {
                    levelStr += "X ";
                }

                //������Σ�����node����(����node����Ϊ�գ�Ҳ����˵�ýڵ㲻���ɿսڵ�����������)
                MAVLTreeNode leftNode = node == null ? null : node.left;
                MAVLTreeNode rightNode = node == null ? null : node.right;
                queue.Enqueue(leftNode);
                queue.Enqueue(rightNode);

                //ֻҪ��һ������һ���ӽڵ㣬��˵��������ײ�
                if (leftNode != null || rightNode != null)
                {
                    isNotBottom = true;
                }
            }
            MLog.Print(MLog.ColorWord("---���������ӻ�---", UnityEngine.Color.black, true, false) + levelStr);
        }

        public static void SortPrint(this MAVLTree tree)
        {
            MLog.Print("���: ");

            if (tree.Count == 0)
            {
                MLog.Print("��Ԫ��");
                return;
            }

            string outputStr = "";
            foreach (var item in tree)
            {
                outputStr += $"{item} ";
            }
            MLog.Print(outputStr);
        }
    }
}
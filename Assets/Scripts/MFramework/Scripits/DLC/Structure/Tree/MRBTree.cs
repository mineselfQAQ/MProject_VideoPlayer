using MFramework;
using MFramework.DLC;
using System;
using System.Collections;

public enum RBColor
{
    Black,
    Red
}

public class MRBTreeNode
{
    #region Internal Field
    internal MRBTree list;

    internal object item;

    internal MRBTreeNode parent;

    internal MRBTreeNode left;

    internal MRBTreeNode right;

    internal RBColor color;
    #endregion

    #region Public Field
    public MRBTree List => list;

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

    public MRBTreeNode Parent => parent;

    public MRBTreeNode Left => left;

    public MRBTreeNode Right => right;

    /// <summary>
    /// ��ɫֵ---��/��
    /// </summary>
    public RBColor Color => color;
    #endregion

    #region Constructor
    public MRBTreeNode(object value)
    {
        item = value;
    }

    internal MRBTreeNode(object value, MRBTree list)
    {
        this.list = list;
        item = value;
    }
    #endregion

    #region Function
    //ʹ�ڵ���Ч(û���������������ϵ����GC����)
    internal void Invalidate()
    {
        //���ڵ�Ͽ�
        if (parent.left == this) parent.left = null;
        else parent.right = null;

        //����Ͽ�
        list = null;//����---list��ս�ͨ����ValidateNode()���Ӷ��޷������κβ���
        parent = null;
        left = null;
        right = null;
    }
    #endregion
}

public class MRBTree : IEnumerable
{
    #region Internal Field
    private MRBTreeNode root;

    private int count;
    #endregion


    #region Public Field
    public MRBTreeNode Root => root;

    public int Count => count;
    #endregion

    #region Constructor
    public MRBTree() { }
    #endregion

    #region Function
    private MRBTreeNode GetParent(MRBTreeNode node) => node?.parent;
    private MRBTreeNode GetGrandParent(MRBTreeNode node) => GetParent(GetParent(node));
    /// <summary>
    /// ��ȡ���ڵ���ֵܽڵ�(������ڵ�)
    /// </summary>
    private MRBTreeNode GetUncle(MRBTreeNode node)
    {
        //�����游�ڵ�
        var gpNode = GetGrandParent(node);
        if (gpNode == null) return null;

        //�游�ڵ��뵱ǰ�ڵ����ӵ���һ���������ڵ�
        if (gpNode.left == node.parent)
        {
            return gpNode.right;
        }
        else
        {
            return gpNode.left;
        }
    }
    private MRBTreeNode GetBrother(MRBTreeNode node)
    {
        var pNode = GetParent(node);
        if (pNode == null) return null;

        if (pNode.right == node)//����
        {
            return pNode?.left;//�ֵܽڵ�������
        }
        else//����
        {
            return pNode?.right;//�ֵܽڵ�������
        }
    }

    private bool isRed(MRBTreeNode node) => node.color == RBColor.Red;
    private bool isBlack(MRBTreeNode node)
    {
        if (node == null) return true;//Ҷ�ӽڵ�(��Ҷ�ӽڵ���ӽڵ�)�ض�Ϊ��ɫ

        return node.color == RBColor.Black;
    }
    private void SetRed(MRBTreeNode node)
    {
        if (node == null) return;
        node.color = RBColor.Red;
    }
    private void SetBlack(MRBTreeNode node)
    {
        if (node == null) return;
        node.color = RBColor.Black;
    }

    public void Add(object o)
    {
        var node = InternalAdd(o);
        AddFix(node);
        UpdateRoot();
        count++;
    }
    public void Remove(object o)
    {
        if (root == null) throw new Exception();//rootΪ����ô��������������Ҫremove

        var node = InternalRemove(o);//����ɾ����node
        RemoveFix(node);//����ṹ
        UpdateRoot();
        node.Invalidate();//������ɾ��
        count--;
    }

    /// <returns>�����Node</returns>
    private MRBTreeNode InternalAdd(object o)
    {
        MRBTreeNode newNode = new MRBTreeNode(o, this);

        if (root == null)
        {
            root = newNode;
            SetBlack(newNode);
            return newNode;
        }

        IComparable num = newNode.item as IComparable;
        if (num == null) throw new Exception();//����û��IComparable�����ݲ��ܱȽ�

        MRBTreeNode cur = root;

        while (true)
        {
            if (num.CompareTo(cur.item) > 0)//num > cur.item
            {
                if (cur.right == null)
                {
                    newNode.parent = cur;
                    cur.right = newNode;
                    SetRed(newNode);
                    return newNode;
                }

                cur = cur.right;//����ֵ����������
            }
            else if (num.CompareTo(cur.item) < 0)//num < cur.item
            {
                if (cur.left == null)
                {
                    newNode.parent = cur;
                    cur.left = newNode;
                    SetRed(newNode);
                    return newNode;
                }

                cur = cur.left;//����ֵ��С��������
            }
            else
            {
                throw new Exception();//һ�Ŷ����������в���������ͬ����
            }
        }
    }
    private void AddFix(MRBTreeNode node)
    {
        //1.�ڵ�Ϊ���ڵ㣬�����޸�
        if (root == node) return;

        var pNode = GetParent(node);
        //2.���ڵ�Ϊ��ɫ�������޸�
        if (pNode != null && isBlack(pNode)) return;

        //3.���ڵ�Ϊ��ɫ
        var gpNode = GetParent(pNode);//�ض�����(���ڵ�Ϊ��ɫ����ô�ض��������ɫ�ڵ�)
        var uNode = GetUncle(node);
        //3.1.����ڵ�Ϊ��ɫ(��û����һ���ɫ�ڵ�)
        if (uNode == null || (uNode != null && isBlack(uNode)))
        {
            //3.1.1.LL���
            if (gpNode?.left == pNode && pNode?.left == node)
            {
                //�游�ڵ�����
                var newGPNode = RightRotate(gpNode);
                //Ⱦɫ
                SetBlack(newGPNode);
                SetRed(newGPNode.right);
            }
            //3.1.2.RR���
            else if (gpNode?.right == pNode && pNode?.right == node)
            {
                //�游�ڵ�����
                var newGPNode = LeftRotate(gpNode);
                //Ⱦɫ
                SetBlack(newGPNode);
                SetRed(newGPNode.left);
            }
            //3.1.3.LR���
            else if (gpNode?.left == pNode && pNode?.right == node)
            {
                //���ڵ��������游�ڵ�����
                LeftRotate(pNode);
                var newGPNode = RightRotate(gpNode);
                //Ⱦɫ
                SetBlack(newGPNode);
                SetRed(newGPNode.right);
            }
            //3.1.4.RL���
            else if (gpNode?.right == pNode && pNode?.left == node)
            {
                //���ڵ��������游�ڵ�����
                RightRotate(pNode);
                var newGPNode = LeftRotate(gpNode);
                //Ⱦɫ
                SetBlack(newGPNode);
                SetRed(newGPNode.left);
            }
        }
        //3.2.����ڵ�Ϊ��ɫ
        else if(uNode != null && isRed(uNode))
        {
            SetRed(gpNode);
            SetBlack(gpNode.left);
            SetBlack(gpNode.right);

            AddFix(gpNode);
        }
    }
    private void UpdateRoot()
    {
        while (root.parent != null)
        {
            root = root.parent;
        }
        SetBlack(root);
    }

    private MRBTreeNode InternalRemove(object o)
    {
        var node = Search(o);

        if (node == null)
        {
            throw new Exception();
        }

        //��Ϊ0(ΪҶ�ӽڵ�)
        if (node.left == null && node.right == null)
        {
            return node;
        }
        else//��ΪҶ�ӽڵ㣬��Ҫ���ҵ�ǰ�����̽ڵ�(����ѡ��ǰ��)
        {
            var tempNode = node;

            if (node.left == null)//ֻ�����ӽڵ�
            {
                //��ȡǰ���ڵ�
                node = node.right;
                while (node.left != null)
                {
                    node = node.left;
                }
            }
            else//һ�����
            {
                //��ȡ��̽ڵ�
                node = node.left;
                while (node.right != null)
                {
                    node = node.right;
                }
            }

            //ǰ���ڵ�����ɾ���ڵ�(���任Ϊɾ��ǰ���ڵ�)
            tempNode.item = node.item;

            return node;
        }
    }
    private void RemoveFix(MRBTreeNode node, bool isLeaf = true)
    {
        //1.ɾ���ڵ�Ϊ���ڵ�
        if (root == node)
        {
            //���Ǹ��ڵ㣬����Ҷ�ӽڵ㣬����ζ��ֻ��1���ڵ㣬ֱ����ռ���
            root = null;
            return;
        }

        var pNode = GetParent(node);
        var bNode = GetBrother(node);
        bool black = isBlack(node);//ɾ���ڵ���ɫ
        var cNode = node.left ?? node.right;//��ȡΨһ��ɫ�ڵ�

        //2.1.ɾ���ڵ�Ϊ��ɫ�ڵ�
        //ֻ��ɾ�����ɣ�������⴦��
        //2.2.ɾ���ڵ�Ϊ��ɫ�ڵ�
        if (black)
        {
            //2.2.1.ӵ��0����ɫ�ڵ�
            if (cNode == null || !isLeaf)
            {
                //Tip����ʱ�������ֵܽڵ㣬��������2-3-4��

                //2.2.1.1.�ֵܽڵ�Ϊ��ɫ
                if (isBlack(bNode))
                {
                    //2.2.1.1.1.�޺�ɫ�ڵ�
                    if (isBlack(bNode.left) && isBlack(bNode.right))
                    {
                        //2.2.1.1.1.1.���ڵ�Ϊ��ɫ
                        if (isRed(pNode))
                        {
                            //�޷����ֵܽڵ�"��"�ڵ㣬ֻ������
                            //�������ڵ����粢��ڣ��ֵܽڵ���
                            SetBlack(pNode);
                            SetRed(bNode);
                        }
                        //2.2.1.1.1.2.���ڵ�Ϊ��ɫ
                        else
                        {
                            //ͬ2.2.1.1.1.1
                            SetBlack(pNode);
                            SetRed(bNode);
                            //���ǣ���ʱ����ɾ���ڵ㲢���ֵܽڵ����죬"����·����ɫ�ڵ�����һ��"�����㣬��Ҫ�ݹ鴦��
                            RemoveFix(pNode, false);
                        }
                    }
                    //2.2.1.1.2.��1��/2����ɫ�ڵ�
                    else
                    {
                        //XL
                        if (bNode.left != null && isRed(bNode.left))
                        {
                            //LL
                            if (pNode.left == bNode)
                            {
                                var newPNode = RightRotate(pNode);
                                SetBlack(newPNode.right);
                            }
                            //RL
                            else if(pNode.right == bNode)
                            {
                                RightRotate(bNode);
                                var newPNode = LeftRotate(pNode);
                                SetBlack(newPNode.left);
                            }
                        }
                        //XR
                        else if (bNode.right != null && isRed(bNode.right))
                        {
                            //RR
                            if (pNode.right == bNode)
                            {
                                var newPNode = LeftRotate(pNode);
                                SetBlack(newPNode.left);
                            }
                            //LR
                            else if (pNode.left == bNode)
                            {
                                LeftRotate(bNode);
                                var newPNode = RightRotate(pNode);
                                SetBlack(newPNode.right);
                            }
                        }
                    }
                }
                //2.2.1.2.�ֵܽڵ�Ϊ��ɫ
                else
                {
                    if (pNode.left == bNode)//�ֵ������
                    {
                        var newPNode = RightRotate(pNode);
                        SetBlack(newPNode);
                        SetRed(newPNode.right);
                        RemoveFix(node, isLeaf);
                    }
                    else//�ֵ����Ҳ�
                    {
                        var newPNode = LeftRotate(pNode);
                        SetBlack(newPNode);
                        SetRed(newPNode.left);
                        RemoveFix(node, isLeaf);
                    }
                }
            }
            else
            {
                throw new Exception();
            }
            //2.2.2.ӵ��1����ɫ�ڵ�
            //�����ܷ�������Ϊ��
            //ӵ��1����ɫ�ڵ���ζ���ǽڵ��Ǻ�ɫ�ڵ�ĸ��ڵ㣬��Ҷ�ӽڵ����
            //else
            //{
            //    bool isRight = pNode?.right == node ? true : false;//���ڵ�ָ��
            //    if (isRight)
            //    {
            //        //���ָ��ڵ�ָ��
            //        pNode.right = cNode;
            //        cNode.parent = pNode;
            //        SetBlack(cNode);
            //    }
            //    else
            //    {
            //        //���ָ��ڵ�ָ��
            //        pNode.left = cNode;
            //        cNode.parent = pNode;
            //        SetBlack(cNode);
            //    }
            //}
            //2.2.3.ӵ��2����ɫ�ڵ�
            //�����ܷ�������Ϊ��
            //ӵ��2����ɫ�ڵ���ζ���ǽڵ��Ǻ�ɫ�ڵ�ĸ��ڵ㣬��Ҷ�ӽڵ����
        }
    }

    public bool Contains(object o)
    {
        return Search(o) != null;
    }
    private MRBTreeNode Search(object o)
    {
        IComparable num = o as IComparable;
        if (num == null) throw new Exception();//����û��IComparable�����ݲ��ܱȽ�

        MRBTreeNode cur = root;

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

    private MRBTreeNode RightRotate(MRBTreeNode node)
    {
        MRBTreeNode parentNode = node.parent;
        MRBTreeNode childNode = node.left;

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
            MRBTreeNode grandChildNode = childNode.right;
            childNode.parent = node.parent;
            childNode.right = node;
            node.left = grandChildNode;
            node.parent = childNode;
            grandChildNode.parent = node;
        }

        //���ø��ڵ���ӽڵ�
        if (parentNode == null) return childNode;
        bool isRight = parentNode.right == node ? true : false;
        if (isRight)//��������
        {
            parentNode.right = childNode;
        }
        else//��������
        {
            parentNode.left = childNode;
        }

        return childNode;//Ϊ���º��"���ڵ�"
    }
    private MRBTreeNode LeftRotate(MRBTreeNode node)
    {
        MRBTreeNode parentNode = node.parent;
        MRBTreeNode childNode = node.right;

        //�����ϲ��ᷢ��������������ӽڵ㶼û�ҵ�����ô�Ͳ���������
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
            MRBTreeNode grandChildNode = childNode.left;
            childNode.parent = node.parent;
            childNode.left = node;
            node.right = grandChildNode;
            node.parent = childNode;
            grandChildNode.parent = node;
        }

        //���ø��ڵ���ӽڵ�
        if (parentNode == null) return childNode;
        bool isRight = parentNode.right == node ? true : false;
        if (isRight)//��������
        {
            parentNode.right = childNode;
        }
        else//��������
        {
            parentNode.left = childNode;
        }

        return childNode;//Ϊ���º��"���ڵ�"
    }

    private void ValidateNode(MRBTreeNode node)
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
    private int InternalInOrder(MRBTreeNode root, object[] list, int index)
    {
        if (root == null) return index - 1;
        int next = InternalInOrder(root.left, list, index);
        next++;
        list[next] = root.item;
        return InternalInOrder(root.right, list, next + 1);
    }
    #endregion
}

public static class MRBTreeExtension
{
    public static void Print(this MRBTree tree)
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
            MRBTreeNode node = (MRBTreeNode)queue.Dequeue();
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
            MRBTreeNode leftNode = node == null ? null : node.left;
            MRBTreeNode rightNode = node == null ? null : node.right;
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

    public static void SortPrint(this MRBTree tree)
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

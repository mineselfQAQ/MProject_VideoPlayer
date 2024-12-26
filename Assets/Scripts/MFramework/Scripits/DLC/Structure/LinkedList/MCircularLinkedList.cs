using System;
using System.Collections;

namespace MFramework.DLC
{
    public class MCircularLinkedListNode
    {
        internal MCircularLinkedList list;

        internal MCircularLinkedListNode next;

        internal object item;

        public MCircularLinkedList List => list;

        public MCircularLinkedListNode Next
        {
            get
            {
                if (next != null)
                {
                    return next;
                }
                return null;
            }
        }

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

        //public�Ĺ��캯�����ɹ�����ṩNodeʹ��
        public MCircularLinkedListNode(object value)
        {
            item = value;
        }
        //internal�Ĺ��캯�����ɹ��ڲ�����������Node
        internal MCircularLinkedListNode(MCircularLinkedList list, object value)
        {
            this.list = list;
            item = value;
        }

        //ʹ�ڵ���Ч(û���������������ϵ����GC����)
        internal void Invalidate()
        {
            list = null;
            next = null;
        }
    }

    public class MCircularLinkedList : IEnumerable
    {
        private MCircularLinkedListNode head;
        internal MCircularLinkedListNode tail;

        public MCircularLinkedListNode First => head;
        public MCircularLinkedListNode Last => tail;

        private int count;
        public int Count => count;

        public MCircularLinkedList() { }

        public MCircularLinkedListNode AddFirst(object value)
        {
            MCircularLinkedListNode newNode = new MCircularLinkedListNode(this, value);

            if (head == null)//��û�нڵ㣬����ͷ��
            {
                InternalInsertNodeEmpty(newNode);
            }
            else//����ͷ�ڵ�ǰ��
            {
                InternalInsertNodeFirst(newNode);
            }

            return newNode;
        }
        public MCircularLinkedListNode AddFirst(MCircularLinkedListNode newNode)
        {
            ValidateNewNode(newNode);

            if (head == null)//��û�нڵ㣬����ͷ��
            {
                InternalInsertNodeEmpty(newNode);
            }
            else//����ͷ�ڵ�ǰ��
            {
                InternalInsertNodeFirst(newNode);
            }

            newNode.list = this;
            return newNode;
        }
        public MCircularLinkedListNode AddLast(object value)
        {
            MCircularLinkedListNode newNode = new MCircularLinkedListNode(this, value);

            if (head == null)//��û�нڵ㣬����ͷ��
            {
                InternalInsertNodeEmpty(newNode);
            }
            else//����β�ڵ����
            {
                InternalInsertNodeLast(newNode);
            }

            return newNode;
        }
        public MCircularLinkedListNode AddLast(MCircularLinkedListNode newNode)
        {
            ValidateNewNode(newNode);

            if (head == null)//��û�нڵ㣬����ͷ��
            {
                InternalInsertNodeEmpty(newNode);
            }
            else//����β�ڵ����
            {
                InternalInsertNodeLast(newNode);
            }

            newNode.list = this;
            return newNode;
        }
        public MCircularLinkedListNode AddBefore(MCircularLinkedListNode node, object value)
        {
            ValidateNode(node);

            MCircularLinkedListNode newNode = new MCircularLinkedListNode(node.list, value);
            InternalInsertNodeBefore(node, newNode);

            return newNode;
        }
        public MCircularLinkedListNode AddBefore(MCircularLinkedListNode node, MCircularLinkedListNode newNode)
        {
            ValidateNode(node);
            ValidateNewNode(newNode);

            InternalInsertNodeBefore(node, newNode);

            newNode.list = this;
            return newNode;
        }
        public MCircularLinkedListNode AddAfter(MCircularLinkedListNode node, object value)
        {
            ValidateNode(node);

            MCircularLinkedListNode newNode = new MCircularLinkedListNode(node.list, value);
            InternalInsertNodeAfter(node, newNode);

            return newNode;
        }
        public MCircularLinkedListNode AddAfter(MCircularLinkedListNode node, MCircularLinkedListNode newNode)
        {
            ValidateNode(node);
            ValidateNewNode(newNode);

            InternalInsertNodeAfter(node, newNode);

            newNode.list = this;
            return newNode;
        }

        public void RemoveFirst()
        {
            if (head == null)
            {
                throw new Exception();
            }

            InternalRemoveNode(head);
        }
        public void RemoveLast()
        {
            if (head == null)
            {
                throw new Exception();
            }

            InternalRemoveNode(tail);
        }
        public bool Remove(object o)
        {
            MCircularLinkedListNode tempNode = Find(o);

            if (tempNode == null) return false;

            InternalRemoveNode(tempNode);
            return true;
        }

        public MCircularLinkedListNode Find(object o)
        {
            if (head == null) return null;

            MCircularLinkedListNode tempNode = head;
            //EqualityComparer<object> @default = EqualityComparer<object>.Default;

            if (o != null)
            {
                do
                {
                    //������==��������Equals()
                    if (tempNode.Value.Equals(o))
                    {
                        return tempNode;
                    }

                    tempNode = tempNode.next;
                }
                while (tempNode != null);
            }
            else
            {
                do
                {
                    if (tempNode.item == null)
                    {
                        return tempNode;
                    }

                    tempNode = tempNode.next;
                }
                while (tempNode != null);
            }

            return null;
        }
        public bool Contains(object o)
        {
            return Find(o) != null;
        }

        public void Clear()
        {
            //�ڴ��ϵ�Clear
            MCircularLinkedListNode nextNode = head;
            int tempCount = count;
            while (tempCount != 0)
            {
                MCircularLinkedListNode tempNode = nextNode;
                nextNode = nextNode.next;
                tempNode.Invalidate();
                tempCount--;
            }

            head = null;
            tail = null;
            count = 0;
        }

        private void ValidateNode(MCircularLinkedListNode node)
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
        private void ValidateNewNode(MCircularLinkedListNode node)
        {
            //�ڵ�������
            if (node == null)
            {
                throw new Exception();
            }
            //�½ڵ��listӦ��Ϊnull
            if (node.list != null)
            {
                throw new Exception();
            }
        }

        //Tip:���ͷ�ڵ㻹��β�ڵ��б䶯������ѭ��������˵����Ҫ����������β����
        private void InternalInsertNodeEmpty(MCircularLinkedListNode newNode)
        {
            head = newNode;
            tail = newNode;
            tail.next = head;//ѭ������
            count++;
        }
        private void InternalInsertNodeFirst(MCircularLinkedListNode newNode)
        {
            newNode.next = head;
            head = newNode;
            tail.next = head;//ѭ������
            count++;
        }
        private void InternalInsertNodeLast(MCircularLinkedListNode newNode)
        {
            tail.next = newNode;
            tail = newNode;
            tail.next = head;//ѭ������
            count++;
        }
        private void InternalInsertNodeBefore(MCircularLinkedListNode node, MCircularLinkedListNode newNode)
        {
            MCircularLinkedListNode beforeNode = FindBeforeNode(node);

            //�������---����λ�þ���ͷ�ڵ�
            if (beforeNode == null)
            {
                newNode.next = head;
                head = newNode;
                tail.next = head;//ѭ������
                count++;
                return;
            }

            //һ�����---�������
            newNode.next = node;
            beforeNode.next = newNode;
            count++;
        }
        private void InternalInsertNodeAfter(MCircularLinkedListNode node, MCircularLinkedListNode newNode)
        {
            //�������---����λ�þ���β�ڵ�
            if (node == tail)
            {
                tail.next = newNode;
                tail = newNode;
                tail.next = head;//ѭ������
                count++;
                return;
            }

            //һ�����---�������
            newNode.next = node.next;
            node.next = newNode;
            count++;
        }
        private void InternalRemoveNode(MCircularLinkedListNode node)
        {
            if (head == tail)//ֻ��һ��Ԫ�����
            {
                head = null;
                tail = null;
            }
            else
            {
                if (node == head)
                {
                    head = head.next;
                    tail.next = head;//ѭ������
                }
                else if (node == tail)
                {
                    MCircularLinkedListNode beforeNode = FindBeforeNode(node);
                    tail = beforeNode;
                    tail.next = head;//ѭ������
                }
                else
                {
                    MCircularLinkedListNode beforeNode = FindBeforeNode(node);
                    beforeNode.next = node.next;
                }
            }

            node.Invalidate();
            count--;
        }

        private MCircularLinkedListNode FindBeforeNode(MCircularLinkedListNode node)
        {
            //�����ҽڵ��Ѿ��ǵ�һ���ڵ�ʱ��û��ǰһ���ڵ�
            if (node == head) return null;

            //�ҵ�ǰһ���ڵ�
            MCircularLinkedListNode before = head;
            while (before.next != node && before != null)//before!=null���Ա����������
            {
                before = before.next;
            }
            return before;
        }


        public IEnumerator GetEnumerator()
        {
            MCircularLinkedListNode nextNode = head;

            int tempCount = count;

            while (tempCount != 0)
            {
                yield return nextNode.Value;
                nextNode = nextNode.next;
                tempCount--;
            }
        }
    }

    public static class MCircularLinkedListExtension
    {
        public static void Print(this MCircularLinkedList list)
        {
            MLog.Print("���: ");

            if (list.Count == 0)
            {
                MLog.Print("��Ԫ��");
                return;
            }

            string outputStr = "";
            foreach (var item in list)
            {
                outputStr += $"{item} ";
            }
            MLog.Print(outputStr);
        }
    }
}
using System;
using System.Collections;

namespace MFramework.DLC
{
    public class MSinglyLinkedListNode
    {
        internal MSinglyLinkedList list;

        internal MSinglyLinkedListNode next;

        internal object item;

        public MSinglyLinkedList List => list;

        public MSinglyLinkedListNode Next
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
        public MSinglyLinkedListNode(object value)
        {
            item = value;
        }
        //internal�Ĺ��캯�����ɹ��ڲ�����������Node
        internal MSinglyLinkedListNode(MSinglyLinkedList list, object value)
        {
            this.list = list;
            item = value;
        }

        //ʹ�ڵ���Ч(û���������������ϵ����GC����)
        internal void Invalidate()
        {
            //���������ж�ǰNode���Node����ϵ�����Ƿ����Ƴ�����InternalRemoveNode()��
            //����һ��������Ƴ��Ὣǰ��Node��ϵ������ֻ��Ҫ���Լ���������Ϊnull����
            //ֻ���Ƴ�β�ڵ�ʱ���Żᷢ�����ô���ȫ
            //MSinglyLinkedListNode beforeNode = list.FindBeforeNode(this);
            //if (beforeNode != null)
            //{
            //    beforeNode.next = null;
            //}

            list = null;
            next = null;
        }
    }

    public class MSinglyLinkedList : IEnumerable
    {
        private MSinglyLinkedListNode head;
        private MSinglyLinkedListNode tail;

        public MSinglyLinkedListNode First => head;
        public MSinglyLinkedListNode Last => tail;

        private int count;
        public int Count => count;

        public MSinglyLinkedList() { }

        public MSinglyLinkedListNode AddFirst(object value)
        {
            MSinglyLinkedListNode newNode = new MSinglyLinkedListNode(this, value);

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
        public MSinglyLinkedListNode AddFirst(MSinglyLinkedListNode newNode)
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
        public MSinglyLinkedListNode AddLast(object value)
        {
            MSinglyLinkedListNode newNode = new MSinglyLinkedListNode(this, value);

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
        public MSinglyLinkedListNode AddLast(MSinglyLinkedListNode newNode)
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
        public MSinglyLinkedListNode AddBefore(MSinglyLinkedListNode node, object value)
        {
            ValidateNode(node);

            MSinglyLinkedListNode newNode = new MSinglyLinkedListNode(node.list, value);
            InternalInsertNodeBefore(node, newNode);

            return newNode;
        }
        public MSinglyLinkedListNode AddBefore(MSinglyLinkedListNode node, MSinglyLinkedListNode newNode)
        {
            ValidateNode(node);
            ValidateNewNode(newNode);

            InternalInsertNodeBefore(node, newNode);

            newNode.list = this;
            return newNode;
        }
        public MSinglyLinkedListNode AddAfter(MSinglyLinkedListNode node, object value)
        {
            ValidateNode(node);

            MSinglyLinkedListNode newNode = new MSinglyLinkedListNode(node.list, value);
            InternalInsertNodeAfter(node, newNode);

            return newNode;
        }
        public MSinglyLinkedListNode AddAfter(MSinglyLinkedListNode node, MSinglyLinkedListNode newNode)
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
            MSinglyLinkedListNode tempNode = Find(o);

            if (tempNode == null) return false;

            InternalRemoveNode(tempNode);
            return true;
        }
        public bool Remove(MSinglyLinkedListNode node)
        {
            ValidateNode(node);
            InternalRemoveNode(node);

            return true;
        }

        public MSinglyLinkedListNode Find(object o)
        {
            if (head == null) return null;

            MSinglyLinkedListNode tempNode = head;
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
            MSinglyLinkedListNode nextNode = head;
            while (nextNode != null)
            {
                MSinglyLinkedListNode tempNode = nextNode;
                nextNode = nextNode.next;
                tempNode.Invalidate();
            }

            head = null;
            tail = null;
            count = 0;
        }

        private void ValidateNode(MSinglyLinkedListNode node)
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
        private void ValidateNewNode(MSinglyLinkedListNode node)
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

        private void InternalInsertNodeEmpty(MSinglyLinkedListNode newNode)
        {
            newNode.next = null;
            head = newNode;
            tail = newNode;
            count++;
        }
        private void InternalInsertNodeFirst(MSinglyLinkedListNode newNode)
        {
            newNode.next = head;
            head = newNode;
            count++;
        }
        private void InternalInsertNodeLast(MSinglyLinkedListNode newNode)
        {
            tail.next = newNode;
            tail = newNode;
            count++;
        }
        private void InternalInsertNodeBefore(MSinglyLinkedListNode node, MSinglyLinkedListNode newNode)
        {
            MSinglyLinkedListNode beforeNode = FindBeforeNode(node);

            //�������---����λ�þ���ͷ�ڵ�
            if (beforeNode == null)
            {
                newNode.next = head;
                head = newNode;
                count++;
                return;
            }

            //һ�����---�������
            newNode.next = node;
            beforeNode.next = newNode;
            count++;
        }
        private void InternalInsertNodeAfter(MSinglyLinkedListNode node, MSinglyLinkedListNode newNode)
        {
            //�������---����λ�þ���β�ڵ�
            if (node == tail)
            {
                tail.next = newNode;
                tail = newNode;
                count++;
                return;
            }

            //һ�����---�������
            newNode.next = node.next;
            node.next = newNode;
            count++;
        }
        private void InternalRemoveNode(MSinglyLinkedListNode node)
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
                }
                else if (node == tail)
                {
                    MSinglyLinkedListNode beforeNode = FindBeforeNode(node);
                    beforeNode.next = null;//��Ҫ�ж�β�ڵ���ǰ�ڵ����ϵ
                    tail = beforeNode;
                }
                else
                {
                    MSinglyLinkedListNode beforeNode = FindBeforeNode(node);
                    beforeNode.next = node.next;
                }
            }

            node.Invalidate();
            count--;
        }

        internal MSinglyLinkedListNode FindBeforeNode(MSinglyLinkedListNode node)
        {
            //�����ҽڵ��Ѿ��ǵ�һ���ڵ�ʱ��û��ǰһ���ڵ�
            if (node == head) return null;

            //�ҵ�ǰһ���ڵ�
            MSinglyLinkedListNode before = head;
            while (before.next != node && before != null)//before!=null���Ա����������
            {
                before = before.next;
            }
            return before;
        }

        public IEnumerator GetEnumerator()
        {
            MSinglyLinkedListNode nextNode = head;
            while (nextNode != null)
            {
                yield return nextNode.Value;
                nextNode = nextNode.next;
            }
        }
    }

    public static class MSinglyLinkedListExtension
    {
        public static void Print(this MSinglyLinkedList list)
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
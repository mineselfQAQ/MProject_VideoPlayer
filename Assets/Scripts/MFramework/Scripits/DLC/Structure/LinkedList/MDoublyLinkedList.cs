using System;
using System.Collections;

namespace MFramework.DLC
{
    public class MDoublyLinkedListNode
    {
        internal MDoublyLinkedList list;

        internal MDoublyLinkedListNode next;

        internal MDoublyLinkedListNode prev;

        internal object item;

        public MDoublyLinkedList List => list;

        public MDoublyLinkedListNode Next
        {
            get
            {
                //���ƣ���������һNode�Ҹ�Node����β�ڵ�
                //��ֹ��ѭ����������������������ģ�
                //��β�ڵ����Next������
                //�������ȡ������ô��õ�ͷ�ڵ㣬����������ǲ���ģ�����ֱ�ӻ�ȡFirst
                //���������ȡ������ô��õ�null�����ǿ����ø����������жϸýڵ��Ƿ�Ϊβ�ڵ�
                if (next != null && next != list.head)
                {
                    return next;
                }
                return null;
            }
        }

        public MDoublyLinkedListNode Previous
        {
            get
            {
                //ͬNext
                if (prev != null && this != list.head)
                {
                    return prev;
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
        public MDoublyLinkedListNode(object value)
        {
            item = value;
        }
        //internal�Ĺ��캯�����ɹ��ڲ�����������Node
        internal MDoublyLinkedListNode(MDoublyLinkedList list, object value)
        {
            this.list = list;
            item = value;
        }

        //ʹ�ڵ���Ч(û���������������ϵ����GC����)
        internal void Invalidate()
        {
            list = null;
            next = null;
            prev = null;
        }
    }

    public class MDoublyLinkedList : IEnumerable
    {
        internal MDoublyLinkedListNode head;

        public MDoublyLinkedListNode First => head;
        public MDoublyLinkedListNode Last
        {
            get
            {
                if (head != null)//ͷ�ڵ㶼û�оͲ�����ͷ�ڵ��ǰһ���ڵ�
                {
                    return head.prev;
                }
                return null;
            }
        }

        private int count;
        public int Count => count;

        public MDoublyLinkedList() { }

        public MDoublyLinkedListNode AddFirst(object value)
        {
            MDoublyLinkedListNode newNode = new MDoublyLinkedListNode(this, value);

            if (head == null)//��û�нڵ㣬����ͷ��
            {
                InternalInsertNodeEmpty(newNode);
            }
            else//����ͷ�ڵ�ǰ��
            {
                InternalInsertNodeBefore(head, newNode);
                head = newNode;
            }

            return newNode;
        }
        public MDoublyLinkedListNode AddFirst(MDoublyLinkedListNode newNode)
        {
            ValidateNewNode(newNode);

            if (head == null)//��û�нڵ㣬����ͷ��
            {
                InternalInsertNodeEmpty(newNode);
            }
            else//����ͷ�ڵ�ǰ��
            {
                InternalInsertNodeBefore(head, newNode);
                head = newNode;
            }

            newNode.list = this;
            return newNode;
        }
        public MDoublyLinkedListNode AddLast(object value)
        {
            MDoublyLinkedListNode newNode = new MDoublyLinkedListNode(this, value);

            if (head == null)//��û�нڵ㣬����ͷ��
            {
                InternalInsertNodeEmpty(newNode);
            }
            else//����β�ڵ����
            {
                InternalInsertNodeBefore(head, newNode);//β�ڵ������ʵ����ͷ�ڵ�ǰ��
            }

            return newNode;
        }
        public MDoublyLinkedListNode AddLast(MDoublyLinkedListNode newNode)
        {
            ValidateNewNode(newNode);

            if (head == null)//��û�нڵ㣬����ͷ��
            {
                InternalInsertNodeEmpty(newNode);
            }
            else//����β�ڵ����
            {
                InternalInsertNodeBefore(head, newNode);//β�ڵ������ʵ����ͷ�ڵ�ǰ��
            }

            newNode.list = this;
            return newNode;
        }
        public MDoublyLinkedListNode AddBefore(MDoublyLinkedListNode node, object value)
        {
            ValidateNode(node);

            MDoublyLinkedListNode newNode = new MDoublyLinkedListNode(node.list, value);
            InternalInsertNodeBefore(node, newNode);

            //�������---�嵽ͷ�ڵ�ǰ����Ҫ�����µ�ͷ�ڵ�
            if (node == head)
            {
                head = newNode;
            }

            return newNode;
        }
        public MDoublyLinkedListNode AddBefore(MDoublyLinkedListNode node, MDoublyLinkedListNode newNode)
        {
            ValidateNode(node);
            ValidateNewNode(newNode);

            InternalInsertNodeBefore(node, newNode);
            newNode.list = this;

            //�������---�嵽ͷ�ڵ�ǰ����Ҫ�����µ�ͷ�ڵ�
            if (node == head)
            {
                head = newNode;
            }

            return newNode;
        }
        public MDoublyLinkedListNode AddAfter(MDoublyLinkedListNode node, object value)
        {
            ValidateNode(node);

            MDoublyLinkedListNode newNode = new MDoublyLinkedListNode(node.list, value);
            InternalInsertNodeBefore(node.next, newNode);//�嵽���棬��ʵ���ǲ嵽��ڵ��ǰ��

            return newNode;
        }
        public MDoublyLinkedListNode AddAfter(MDoublyLinkedListNode node, MDoublyLinkedListNode newNode)
        {
            ValidateNode(node);
            ValidateNewNode(newNode);

            InternalInsertNodeBefore(node.next, newNode);//�嵽���棬��ʵ���ǲ嵽��ڵ��ǰ��
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

            InternalRemoveNode(head.prev);
        }
        public bool Remove(object o)
        {
            MDoublyLinkedListNode tempNode = Find(o);

            if (tempNode == null) return false;

            InternalRemoveNode(tempNode);
            return true;
        }
        public bool Remove(MDoublyLinkedListNode node)
        {
            ValidateNode(node);
            InternalRemoveNode(node);

            return true;
        }

        public MDoublyLinkedListNode Find(object o)
        {
            if (head == null) return null;

            MDoublyLinkedListNode tempNode = head;
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
                while (tempNode != head);
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
                while (tempNode != head);
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
            MDoublyLinkedListNode nextNode = head;
            while (nextNode != null)
            {
                MDoublyLinkedListNode tempNode = nextNode;
                nextNode = nextNode.Next;//����Ϊ����Next���Է�ֹѭ��
                tempNode.Invalidate();
            }

            head = null;
            count = 0;
        }

        private void ValidateNode(MDoublyLinkedListNode node)
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
        private void ValidateNewNode(MDoublyLinkedListNode node)
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

        private void InternalInsertNodeEmpty(MDoublyLinkedListNode newNode)
        {
            newNode.next = newNode;//��ѭ��
            newNode.prev = newNode;//��ѭ��
            head = newNode;
            count++;
        }
        private void InternalInsertNodeBefore(MDoublyLinkedListNode node, MDoublyLinkedListNode newNode)
        {
            newNode.next = node;
            newNode.prev = node.prev;
            node.prev.next = newNode;
            node.prev = newNode;

            count++;
        }
        private void InternalRemoveNode(MDoublyLinkedListNode node)
        {
            if (node.next == node)//ֻ��һ���ڵ�
            {
                head = null;
            }
            else
            {
                node.next.prev = node.prev;
                node.prev.next = node.next;

                //�������---ɾ���ڵ�Ϊͷ�ڵ㣬��ô�µ�ͷ�ڵ������һ���ڵ�
                if (head == node)
                {
                    head = node.next;
                }
            }

            node.Invalidate();
            count--;
        }

        public IEnumerator GetEnumerator()
        {
            MDoublyLinkedListNode nextNode = head;
            while (nextNode != null)
            {
                yield return nextNode.Value;
                nextNode = nextNode.Next;
            }
        }
    }

    public static class MDoublyLinkedListExtension
    {
        public static void Print(this MDoublyLinkedList list)
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
using System;
using System.Collections.Generic;

namespace MFramework.DLC
{
    public static class SortingAlgorithm
    {
        /// <summary>
        /// <para>ѡ������---����ÿһ��ѭ����ѡ��һ����СԪ�أ�������õ�����������ĺ���</para>
        /// ������Ӧ����ʱ�临�Ӷ�O(n^2) | ԭ�����򣬿ռ临�Ӷ�O(1) | ���ȶ�����
        /// </summary>
        public static void SelectionSort(int[] arr)
        {
            //��Ҫ����(����-1)�Σ���Ϊֻʣһ��Ԫ��ʱ����������
            for (int i = 0; i < arr.Length - 1; i++)
            {
                int min = i;
                //Ѱ����СԪ��
                for (int j = i + 1; j < arr.Length; j++)
                {
                    if (arr[j] < arr[min])
                    {
                        min = j;
                    }
                }
                //����
                Swap(ref arr[i], ref arr[min]);
            }
        }

        /// <summary>
        /// <para>ð������---�����رȽ��뽻������Ԫ��ʵ������</para>
        /// ����Ӧ����ʱ�临�Ӷ�O(n^2) | ԭ�����򣬿ռ临�Ӷ�O(1) | �ȶ�����
        /// </summary>
        public static void BubbleSort(int[] arr)
        {
            //ÿ�ֽ��ж�һ��Ԫ�ص�"ð��"����
            for(int i = 0; i < arr.Length - 1; i++)
            {
                bool flag = false;

                //��ǰ����ÿ����Ԫ�ؽ��н����Ӷ�"ð��"
                for (int j = 0; j < arr.Length - 1 - i; j++)
                {
                    if (arr[j] > arr[j + 1])
                    {
                        Swap(ref arr[j], ref arr[j + 1]);
                        flag = true;
                    }
                }

                if (!flag) break;//���һ��ѭ����û�н��н���������˵����������ɣ��˳�����
            }
        }

        /// <summary>
        /// <para>��������---ÿ�ν�һ��δ����Ԫ�ذ���С��������Ԫ���У�ֱ���������</para>
        /// ����Ӧ����ʱ�临�Ӷ�O(n^2) | ԭ�����򣬿ռ临�Ӷ�O(1) | �ȶ�����
        /// </summary>
        public static void InsertionSort(int[] arr)
        {
            for (int i = 1; i < arr.Length; i++)
            {
                int temp = arr[i];//�ݴ�ֵ
                int j = i - 1;//index

                //�������ֵ��С��Ϊ��"�ڿռ�"(����)
                while (j >= 0 && arr[j] > temp)
                {
                    arr[j + 1] = arr[j];
                    j--;
                }
                //����ֵ
                arr[j + 1] = temp;
            }
        }

        /// <summary>
        /// <para>ϣ������---���ڲ������򣬻���ݲ�����ν��в�������ÿ�β����ʹ��������</para>
        /// ����Ӧ����(Ӧ��)��ʱ�临�Ӷ�O(nlogn) | ԭ�����򣬿ռ临�Ӷ�O(1) | ���ȶ�����
        /// </summary>
        public static void ShellSort(int[] arr)
        {
            int gap = arr.Length;//gap---ÿ��Ԫ��֮��ļ��

            while (gap != 1)//���һ�μ����Ϊ1��Ҳ������������
            {
                gap /= 2;//������٣�Ҳ����ÿ������Ԫ�����ӣ����ռ��Ϊ1����������

                //��������
                for (int i = 0; i < gap; i++)
                {
                    //ʹ�ò�����������
                    for (int j = i; j < arr.Length; j += gap)
                    {
                        int temp = arr[j];//�ݴ�ֵ
                        int k = j - 1;//index

                        //�������ֵ��С��Ϊ��"�ڿռ�"(����)
                        while (k >= 0 && arr[k] > temp)
                        {
                            arr[k + 1] = arr[k];
                            k--;
                        }
                        //����ֵ
                        arr[k + 1] = temp;
                    }
                }
            }
        }

        /// <summary>
        /// <para>��������---���δ���ÿ�ζ�����������|���<��ǰ<�Ҳ�|���ݹ���һ��Ԫ���������</para>
        /// ����Ӧ����ʱ�临�Ӷ�O(nlogn) | ԭ�����򣬿ռ临�Ӷ�O(n) | ���ȶ�����
        /// </summary>
        public static void QuickSort(int[] arr)
        {
            QuickSort(arr, 0, arr.Length - 1);
        }
        private static void QuickSort(int[] arr, int left, int right)
        {
            while (left < right)
            {
                int pivot = Partition(arr, left, right);

                //ֻ�Զ̵��Ǹ����еݹ�
                if (pivot - left < right - pivot)
                {
                    QuickSort(arr, left, pivot - 1);//��̣���right
                    left = pivot + 1;//��һ�룬��while��䴦��
                }
                else
                {
                    QuickSort(arr, pivot + 1, right);//�Ҷ̣���left
                    right = pivot - 1;//��һ�룬��while��䴦��
                }
            }
        }
        private static int Partition(int[] arr, int left, int right)
        {
            int median = MedianThree(arr, left, (left + right) / 2, right);
            Swap(ref arr[left], ref arr[median]);

            int l = left, r = right;
            while (l < r)//���ֻ��һ��Ԫ�ز���Ҫ����
            {
                //Ѱ�Ҳ�����Ԫ������(���Ĵ�Ԫ�أ��Ҳ��СԪ��)
                //Tip������֮���൱���ּ����һ�Σ��������ڻ��ˣ��˴αض��ᷢ��һ��r--��l++
                while (l < r && arr[r] >= arr[left])
                {
                    r--;
                }
                while (l < r && arr[l] <= arr[left])
                {
                    l++;
                }
                Swap(ref arr[l], ref arr[r]);//�����������ϵ�Ԫ�ؽ���λ��(ָ���غϴ�Ϊ�ֽ�㣬���������)
            }
            Swap(ref arr[l], ref arr[left]);//����׼ֵ�������ָ�λ��
            return l;
        }
        private static int MedianThree(int[] arr, int left, int mid, int right)
        {
            if ((arr[left] < arr[mid] && arr[right] > arr[mid]) || (arr[right] < arr[mid] && arr[left] > arr[mid]))
            {
                return mid;
            }
            else if ((arr[mid] < arr[left] && arr[right] > arr[left]) || (arr[right] < arr[left] && arr[mid] > arr[left]))
            {
                return left;
            }
            else
            {
                return right;
            }
        }

        /// <summary>
        /// <para>�鲢����---���δ���������ݹ��ֳ�������飬�ٲ��Ϻϲ�����������</para>
        /// ������Ӧ����ʱ�临�Ӷ�O(nlogn) | ��ԭ�����򣬿ռ临�Ӷ�O(n) | �ȶ�����
        /// </summary>
        public static void MergeSort(int[] arr)
        {
            MergeSort(arr, 0, arr.Length - 1);
        }
        private static void MergeSort(int[] arr, int left, int right)
        {
            //���ֵ�һ��Ԫ��ʱ�˳�
            if (left >= right) return;

            int mid = (left + right) / 2;
            MergeSort(arr, left, mid);
            MergeSort(arr, mid + 1, right);
            Merge(arr, left, mid, right);
        }
        private static void Merge(int[] arr, int left, int mid, int right)
        {
            int[] tempArr = new int[right - left + 1];//������ʱ���飬��С���ǵ�ǰ�ִεĴ�С

            //i---������j---������k---��ʼ����
            int i = left, j = mid + 1, k = 0;
            //����ע�⣺
            //�����Ǵ�������鿪ʼ�ϲ��ģ�������������һֱ��������ģ�����˵����ͨ��������������ѡȡ�ķ�ʽ��������
            while (i <= mid && j <= right)
            {
                if (arr[i] <= arr[j])
                {
                    tempArr[k++] = arr[i++];
                }
                else
                {
                    tempArr[k++] = arr[j++];
                }
            }
            //�����������ܳ���һ����ȫ��ת�ƣ���һ�໹ʣ�¶��Ԫ�أ���Ҫȫ�����η���
            while (i <= mid)
            {
                tempArr[k++] = arr[i++];
            } 
            while (j <= right)
            {
                tempArr[k++] = arr[j++];
            }

            //����ת�ƻ�ԭ������
            for (k = 0; k < tempArr.Length; k++)
            {
                arr[left + k] = tempArr[k];
            }
        }

        /// <summary>
        /// <para>������---ͨ���󶥶ѣ�ÿ�ν����Ԫ�ط�����β����������Ԫ�ضѻ���֤�󶥶�����</para>
        /// ������Ӧ����ʱ�临�Ӷ�O(nlogn) | ԭ�����򣬿ռ临�Ӷ�O(1) | ���ȶ�����
        /// </summary>
        public static void HeapSort(int[] arr)
        {
            //���������(�ӷ�Ҷ�ӽڵ㿪ʼ)�����жѻ�����
            for (int i = arr.Length / 2 - 1; i >= 0; i--)
            {
                SiftDown(arr, arr.Length, i);
            }

            for (int i = arr.Length - 1; i > 0; i--)
            {
                Swap(ref arr[0], ref arr[i]);//��β����
                SiftDown(arr, i, 0);//��δ����������жѻ�
            }
        }
        private static void SiftDown(int[] arr, int n, int i)
        {
            while (true)
            {
                //max---�ýڵ�������l---��ڵ�������r---�ҽڵ�����
                int max = i;
                int l = 2 * i + 1;
                int r = 2 * i + 2;

                //�ӽڵ�δ���� �� ���ڵ㲢����󣬴�ʱ����max
                if (l < n && arr[l] > arr[max])
                {
                    max = l;
                }
                if (r < n && arr[r] > arr[max])
                {
                    max = r;
                }

                if (max == i)//�������㣬����ִ��(���ڵ�Ϊ���ֵ)
                {
                    break;
                }

                Swap(ref arr[i], ref arr[max]);//�����ڵ��е�ֵ
                i = max;//����һ��
            }
        }

        /// <summary>
        /// <para>Ͱ����---�������ȷ����ڲ�ͬ��Ͱ�У���Ͱ�������ϲ���ԭ����</para>
        /// <para>ע�⣺��������Ϊȡֵ��Χ[0,1)</para>
        /// ����Ӧ����ʱ�临�Ӷ�O(n)(�������) | ��ԭ�����򣬿ռ临�Ӷ�O(n) | �ȶ���ȡ����Ͱ�е������㷨
        /// </summary>
        public static void BucketSort(float[] arr)
        {
            //��ʼ��Ͱ
            int k = arr.Length / 2;//����Ͱ������
            List<List<float>> buckets = new List<List<float>>();
            for (int i = 0; i < k; i++)
            {
                buckets.Add(new List<float>());
            }

            //���������Ͱ��
            foreach (float num in arr)
            {
                //�ؼ�---ԭ����[0,1)������ӳ����[0,k-1](����)
                //����˵��������ֵ�Ĵ�С���Ὣ������ڲ�ͬ��Ͱ��
                int i = (int)(num * k);
                buckets[i].Add(num);
            }

            //��ÿ��Ͱ��������
            foreach (List<float> bucket in buckets)
            {
                bucket.Sort();
            }

            //��Ͱ������������ת�ƻ�������
            int j = 0;
            foreach (List<float> bucket in buckets)
            {
                foreach (float num in bucket)
                {
                    arr[j++] = num;
                }
            }
        }

        /// <summary>
        /// <para>��������(������)---Ͱ�����һ����������ֵ��Ϊ�������δ���һ�����飬ÿ��һ������ʹ��Ӧ�����ļ���+1��
        /// ����ͨ���õ��ļ������鸲����ԭ����</para>
        /// <para>ע�⣺��������Ϊֻ�����ڷǸ�����(�������ת��Ҳ��)</para>
        /// ʱ�临�Ӷ�O(n) | ��ԭ�����򣬿ռ临�Ӷ�O(n) | ���ȶ�����
        /// </summary>
        public static void CountingSortNaive(int[] arr)
        {
            //��ȡ���ֵ
            int max = 0;
            foreach (int num in arr)
            {
                max = Math.Max(max, num);
            }

            //�����С�������������������ֵ��С����Ȼ�������Ϊmax����ô����Ԫ�ظ���Ӧ��Ϊmax+1
            int[] counter = new int[max + 1];
            //��Ԫ�ظ���ֵ�Ĵ�С�����������Ķ�Ӧ������
            foreach (int num in arr)
            {
                counter[num]++;
            }

            //������Ԫ�ذ��򸲸�
            int i = 0;
            for (int num = 0; num < max + 1; num++)//ÿ�鶼��������ͬ��ֵ
            {
                for (int j = 0; j < counter[num]; j++, i++)//ֱ��j���������ʱ���˳���������һ��ֵ
                {
                    arr[i] = num;
                }
            }
        }
        /// <summary>
        /// <para>��������---Ͱ�����һ����������ֵ��Ϊ�������δ���һ�����飬ÿ��һ������ʹ��Ӧ�����ļ���+1��
        /// �ڴ˻���֮�ϸ���Ϊǰ׺�ͣ�����ͨ����������õ�res���鲢������ԭ����</para>
        /// <para>ע�⣺��������Ϊֻ�����ڷǸ�����(�������ת��Ҳ��)</para>
        /// ʱ�临�Ӷ�O(n) | ��ԭ�����򣬿ռ临�Ӷ�O(n) | �ȶ�����
        /// </summary>
        public static void CountingSort(int[] arr)
        {
            //��ȡ���ֵ
            int max = 0;
            foreach (int num in arr)
            {
                max = Math.Max(max, num);
            }

            //�����С�������������������ֵ��С����Ȼ�������Ϊmax����ô����Ԫ�ظ���Ӧ��Ϊmax+1
            int[] counter = new int[max + 1];
            //��Ԫ�ظ���ֵ�Ĵ�С�����������Ķ�Ӧ������
            foreach (int num in arr)
            {
                counter[num]++;
            }
            //�������������Ϊǰ׺������
            for (int i = 0; i < max; i++)
            {
                counter[i + 1] += counter[i];
            }

            int n = arr.Length;
            int[] res = new int[n];//�ݴ�����
            //�������
            for (int i = n - 1; i >= 0; i--)
            {
                int num = arr[i];//ֵ��С��ͬ��Ϊ����
                res[counter[num] - 1] = num;//��ֵ����res����
                counter[num]--;//����ǰ׺��
            }

            //����ԭ����
            for (int i = 0; i < n; i++)
            {
                arr[i] = res[i];
            }
        }

        /// <summary>
        /// <para>��������---һ��ͨ����ÿ��λ����ķ���</para>
        /// <para>ע�⣺���ݱ�����Ա�ʾΪ�̶�λ���ĸ�ʽ����λ�����ܹ���</para>
        /// ʱ�临�Ӷ�O(n) | ��ԭ�����򣬿ռ临�Ӷ�O(n) | �ȶ�����(������������ȶ�)
        /// </summary>
        public static void RadixSort(int[] arr)
        {
            //��ȡ���ֵ
            int max = int.MinValue;
            foreach (int num in arr)
            {
                if (num > max) max = num;
            }

            //���մӵ�λ����λ��˳�����ִ��
            for (int exp = 1; exp <= max; exp *= 10)
            {
                CountingSortDigit(arr, exp);//���÷ǳ���ļ��������������
            }
        }
        private static void CountingSortDigit(int[] arr, int exp)
        {
            int[] counter = new int[10];//ʮ������ֻ��10������Ͱ�Ĵ�С��Ϊ10
            int n = arr.Length;

            //��Ԫ�ظ���ֵ�Ĵ�С�����������Ķ�Ӧ������
            for (int i = 0; i < n; i++)
            {
                int d = Digit(arr[i], exp);//��ȡĳһλ��ֵ
                counter[d]++;
            }
            //�������������Ϊǰ׺������
            for (int i = 1; i < 10; i++)
            {
                counter[i] += counter[i - 1];
            }

            int[] res = new int[n];//�ݴ�����
            //�������
            for (int i = n - 1; i >= 0; i--)
            {
                int d = Digit(arr[i], exp);//ֵ��С��ͬ��Ϊ����
                int j = counter[d] - 1;//��ȡ��������
                res[j] = arr[i];//��ֵ����res����
                counter[d]--;//����ǰ׺��
            }

            //����ԭ����
            for (int i = 0; i < n; i++)
            {
                arr[i] = res[i];
            }
        }
        private static int Digit(int num, int exp)
        {
            return (num / exp) % 10;//��ȡ10������ĳһλ��ֵ
        }

        private static void Swap(ref int a, ref int b)
        {
            int temp = a;
            a = b;
            b = temp;
        }
    }
}

using System;

namespace MFramework
{
    public class MCurve
    {
        public CurveType curveType;
        public CurveDir curveDir;

        public Func<float, float> func { get; private set; }//���ߺ���

        /// <summary>
        /// ���Ե���
        /// </summary>
        public static MCurve Linear { get { return new MCurve(CurveType.Linear); } }

        /// <summary>
        /// �������(���Һ���)
        /// </summary>
        public static MCurve SineIn { get { return new MCurve(CurveType.SineIn); } }
        /// <summary>
        /// �������(���Һ���)
        /// </summary>
        public static MCurve SineOut { get { return new MCurve(CurveType.SineOut); } }
        /// <summary>
        /// ������(���Һ���)
        /// </summary>
        public static MCurve SineInOut { get { return new MCurve(CurveType.SineInOut); } }

        /// <summary>
        /// �������(2�κ���)
        /// </summary>
        public static MCurve QuadIn { get { return new MCurve(CurveType.QuadIn); } }
        /// <summary>
        /// �������(2�κ���)
        /// </summary>
        public static MCurve QuadOut { get { return new MCurve(CurveType.QuadOut); } }
        /// <summary>
        /// ������(2�κ���)
        /// </summary>
        public static MCurve QuadInOut { get { return new MCurve(CurveType.QuadInOut); } }

        /// <summary>
        /// �������(3�κ���)
        /// </summary>
        public static MCurve CubicIn { get { return new MCurve(CurveType.CubicIn); } }
        /// <summary>
        /// �������(3�κ���)
        /// </summary>
        public static MCurve CubicOut { get { return new MCurve(CurveType.CubicOut); } }
        /// <summary>
        /// ������(3�κ���)
        /// </summary>
        public static MCurve CubicInOut { get { return new MCurve(CurveType.CubicInOut); } }

        /// <summary>
        /// �������(4�κ���)
        /// </summary>
        public static MCurve QuartIn { get { return new MCurve(CurveType.QuartIn); } }
        /// <summary>
        /// �������(4�κ���)
        /// </summary>
        public static MCurve QuartOut { get { return new MCurve(CurveType.QuartOut); } }
        /// <summary>
        /// ������(4�κ���)
        /// </summary>
        public static MCurve QuartInOut { get { return new MCurve(CurveType.QuartInOut); } }

        /// <summary>
        /// �������(5�κ���)
        /// </summary>
        public static MCurve QuintIn { get { return new MCurve(CurveType.QuintIn); } }
        /// <summary>
        /// �������(5�κ���)
        /// </summary>
        public static MCurve QuintOut { get { return new MCurve(CurveType.QuintOut); } }
        /// <summary>
        /// ������(5�κ���)
        /// </summary>
        public static MCurve QuintInOut { get { return new MCurve(CurveType.QuintInOut); } }

        /// <summary>
        /// �������(ָ������)
        /// </summary>
        public static MCurve ExpoIn { get { return new MCurve(CurveType.ExpoIn); } }
        /// <summary>
        /// �������(ָ������)
        /// </summary>
        public static MCurve ExpoOut { get { return new MCurve(CurveType.ExpoOut); } }
        /// <summary>
        /// ������(ָ������)
        /// </summary>
        public static MCurve ExpoInOut { get { return new MCurve(CurveType.ExpoInOut); } }

        /// <summary>
        /// �������(Բ�����ߺ���)
        /// </summary>
        public static MCurve CircIn { get { return new MCurve(CurveType.CircIn); } }
        /// <summary>
        /// �������(Բ�����ߺ���)
        /// </summary>
        public static MCurve CircOut { get { return new MCurve(CurveType.CircOut); } }
        /// <summary>
        /// ������(Բ�����ߺ���)
        /// </summary>
        public static MCurve CircInOut { get { return new MCurve(CurveType.CircInOut); } }

        /// <summary>
        /// ������(�������������)
        /// </summary>
        public static MCurve BackIn { get { return new MCurve(CurveType.BackIn); } }
        /// <summary>
        /// �쵯��(�������������)
        /// </summary>
        public static MCurve BackOut { get { return new MCurve(CurveType.BackOut); } }
        /// <summary>
        /// ���뵯��(|0|->|-0.1|->|1.1|->|1|��������)
        /// </summary>
        public static MCurve BackInOut { get { return new MCurve(CurveType.BackInOut); } }


        /// <summary>
        /// �𶯵���(��������)
        /// </summary>
        public static MCurve ElasticIn { get { return new MCurve(CurveType.ElasticIn); } }
        /// <summary>
        /// �𶯵���(��������)
        /// </summary>
        public static MCurve ElasticOut { get { return new MCurve(CurveType.ElasticOut); } }
        /// <summary>
        /// �𶯵��뵯��(��������)
        /// </summary>
        public static MCurve ElasticInOut { get { return new MCurve(CurveType.ElasticInOut); } }

        /// <summary>
        /// �������쵯
        /// </summary>
        public static MCurve BounceIn { get { return new MCurve(CurveType.BounceIn); } }
        /// <summary>
        /// �쵯������
        /// </summary>
        public static MCurve BounceOut { get { return new MCurve(CurveType.BounceOut); } }
        /// <summary>
        /// �����쵯����
        /// </summary>
        public static MCurve BounceInOut { get { return new MCurve(CurveType.BounceInOut); } }

        public MCurve(CurveType curveType, CurveDir curveDir = CurveDir.Increment)//��������
        {
            this.curveType = curveType;
            this.curveDir = curveDir;
        }

        public MCurve(Func<float, float> func, CurveDir curveDir = CurveDir.Increment)//�Զ�������
        {
            this.func = func;
            this.curveDir = curveDir;
        }

        public static MCurve GetMCurve(CurveType type)
        {
            switch (type)
            {
                case CurveType.Linear:
                    return MCurve.Linear;
                case CurveType.QuadIn:
                    return MCurve.QuadIn;
                case CurveType.QuadOut:
                    return MCurve.QuadOut;
                case CurveType.QuadInOut:
                    return MCurve.QuadInOut;
                case CurveType.CubicIn:
                    return MCurve.CubicIn;
                case CurveType.CubicOut:
                    return MCurve.CubicOut;
                case CurveType.CubicInOut:
                    return MCurve.CubicInOut;
                case CurveType.QuartIn:
                    return MCurve.QuartIn;
                case CurveType.QuartOut:
                    return MCurve.QuartOut;
                case CurveType.QuartInOut:
                    return MCurve.QuartInOut;
                case CurveType.QuintIn:
                    return MCurve.QuintIn;
                case CurveType.QuintOut:
                    return MCurve.QuintOut;
                case CurveType.QuintInOut:
                    return MCurve.QuintInOut;
                case CurveType.SineIn:
                    return MCurve.SineIn;
                case CurveType.SineOut:
                    return MCurve.SineOut;
                case CurveType.SineInOut:
                    return MCurve.SineInOut;
                case CurveType.ExpoIn:
                    return MCurve.ExpoIn;
                case CurveType.ExpoOut:
                    return MCurve.ExpoOut;
                case CurveType.ExpoInOut:
                    return MCurve.ExpoInOut;
                case CurveType.ElasticIn:
                    return MCurve.ElasticIn;
                case CurveType.ElasticOut:
                    return MCurve.ElasticOut;
                case CurveType.ElasticInOut:
                    return MCurve.ElasticInOut;
                case CurveType.CircIn:
                    return MCurve.CircIn;
                case CurveType.CircOut:
                    return MCurve.CircOut;
                case CurveType.CircInOut:
                    return MCurve.CircInOut;
                case CurveType.BackIn:
                    return MCurve.BackIn;
                case CurveType.BackOut:
                    return MCurve.BackOut;
                case CurveType.BackInOut:
                    return MCurve.BackInOut;
                case CurveType.BounceIn:
                    return MCurve.BounceIn;
                case CurveType.BounceOut:
                    return MCurve.BounceOut;
                case CurveType.BounceInOut:
                    return MCurve.BounceInOut;
                default:
                    return null;
            }
        }
    }

    public static class MCurveExtension
    {
        /// <summary>
        /// ��ת(0���Ϊ1��)
        /// </summary>
        /// <param playerName="curve"></param>
        /// <returns></returns>
        public static MCurve Reverse(this MCurve curve)
        {
            return new MCurve(curve.curveType, curve.curveDir == CurveDir.Increment ? CurveDir.Decrement : CurveDir.Increment);
        }
    }
}
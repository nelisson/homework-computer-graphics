namespace Utilities.Extension
{
    using System;
    using OpenTK;

    public static class MatrixEx
    {
        public static void Rotate(ref Matrix4 m, float angle, Vector3 axis)
        {
            var s = (float) Math.Sin(MathHelper.DegreesToRadians(angle));
            var c = (float) Math.Cos(MathHelper.DegreesToRadians(angle));

            axis.Normalize();

            float ux = axis.X;
            float uy = axis.Y;
            float uz = axis.Z;

            m.M11 = c + (1 - c)*ux;
            m.M12 = (1 - c)*ux*uy + s*uz;
            m.M13 = (1 - c)*ux*uz - s*uy;
            m.M14 = 0;

            m.M21 = (1 - c)*uy*ux - s*uz;
            m.M22 = c + (1 - c)*(float) Math.Pow(uy, 2);
            m.M23 = (1 - c)*uy*uz + s*ux;
            m.M24 = 0;

            m.M31 = (1 - c)*uz*ux + s*uy;
            m.M32 = (1 - c)*uz*uz - s*ux;
            m.M33 = c + (1 - c)*(float) Math.Pow(uz, 2);
            m.M34 = 0;

            m.M41 = 0;
            m.M42 = 0;
            m.M43 = 0;
            m.M44 = 1;
        }
    }
}
namespace Utilities.Extension
{
    using System;
    using OpenTK;
    using OpenTK.Graphics.OpenGL;

    public static class QuaternionEx
    {
        public static void CustomRotate(Vector3 axis, float angle)
        {
            Matrix4 matrix = new Quaternion(axis.Normalized()*(float) Math.Sin(MathHelper.DegreesToRadians(angle)/2),
                                            (float) Math.Cos(MathHelper.DegreesToRadians(angle)/2)).RotationMatrix();

            GL.MultMatrix(ref matrix);
        }

        private static Matrix4 RotationMatrix(this Quaternion q)
        {
            float q0 = q.W;
            float q1 = q.X;
            float q2 = q.Y;
            float q3 = q.Z;

            return new Matrix4(
                1 - 2*(q2*q2 + q3*q3), 2*(q1*q2 - q0*q3), 2*(q1*q3 + q0*q2), 0,
                2*(q1*q2 + q0*q3), 1 - 2*(q1*q1 + q3*q3), 2*(q2*q3 - q0*q1), 0,
                2*(q1*q3 - q0*q2), 2*(q2*q3 + q0*q1), 1 - 2*(q1*q1 + q2*q2), 0,
                0, 0, 0, 1);
        }
    }
}
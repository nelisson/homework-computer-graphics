#pragma warning disable 612,618
namespace Utilities
{
    using System;
    using System.Drawing;
    using OpenTK;
    using OpenTK.Graphics;
    using OpenTK.Graphics.OpenGL;
    using AttribMask = OpenTK.Graphics.OpenGL.AttribMask;
    using BeginMode = OpenTK.Graphics.OpenGL.BeginMode;
    using ColorPointerType = OpenTK.Graphics.OpenGL.ColorPointerType;
    using DrawElementsType = OpenTK.Graphics.OpenGL.DrawElementsType;
    using EnableCap = OpenTK.Graphics.OpenGL.EnableCap;
    using GL = OpenTK.Graphics.OpenGL.GL;
    using MapTarget = OpenTK.Graphics.OpenGL.MapTarget;
    using MaterialFace = OpenTK.Graphics.OpenGL.MaterialFace;
    using MeshMode2 = OpenTK.Graphics.MeshMode2;
    using NormalPointerType = OpenTK.Graphics.OpenGL.NormalPointerType;
    using PolygonMode = OpenTK.Graphics.OpenGL.PolygonMode;
    using VertexPointerType = OpenTK.Graphics.OpenGL.VertexPointerType;

    public static class Elements
    {
        /* Rim, body, lid, and bottom data must be reflected in x and
   y; handle and spout data across the y axis only.  */

        private static readonly int[,] Patchdata = new[,]
                                                       {
                                                           /* rim */
                                                           {
                                                               102, 103, 104, 105, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14,
                                                               15
                                                           },
                                                           /* body */
                                                           {
                                                               12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25,
                                                               26,
                                                               27
                                                           },
                                                           {
                                                               24, 25, 26, 27, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38,
                                                               39,
                                                               40
                                                           },
                                                           /* lid */
                                                           {
                                                               96, 96, 96, 96, 97, 98, 99, 100, 101, 101, 101, 101, 0, 1
                                                               , 2
                                                               , 3
                                                           },
                                                           {
                                                               0, 1, 2, 3, 106, 107, 108, 109, 110, 111, 112, 113, 114,
                                                               115
                                                               , 116, 117
                                                           },
                                                           /* bottom */
                                                           {
                                                               118, 118, 118, 118, 124, 122, 119, 121, 123, 126, 125,
                                                               120,
                                                               40, 39, 38, 37
                                                           },
                                                           /* handle */
                                                           {
                                                               41, 42, 43, 44, 45, 46, 47, 48, 49, 50, 51, 52, 53, 54,
                                                               55,
                                                               56
                                                           },
                                                           {
                                                               53, 54, 55, 56, 57, 58, 59, 60, 61, 62, 63, 64, 28, 65,
                                                               66,
                                                               67
                                                           },
                                                           /* spout */
                                                           {
                                                               68, 69, 70, 71, 72, 73, 74, 75, 76, 77, 78, 79, 80, 81,
                                                               82,
                                                               83
                                                           },
                                                           {
                                                               80, 81, 82, 83, 84, 85, 86, 87, 88, 89, 90, 91, 92, 93,
                                                               94,
                                                               95
                                                           }
                                                       };

        private static readonly double[,] Cpdata = new[,]
                                                       {
                                                           {0.2, 0, 2.7}, {0.2, -0.112, 2.7}, {0.112, -0.2, 2.7}, {
                                                                                                                      0,
                                                                                                                      -0.2
                                                                                                                      ,
                                                                                                                      2.7
                                                                                                                  },
                                                           {1.3375, 0, 2.53125}, {1.3375, -0.749, 2.53125},
                                                           {0.749, -1.3375, 2.53125}, {0, -1.3375, 2.53125}, {
                                                                                                                 1.4375,
                                                                                                                 0,
                                                                                                                 2.53125
                                                                                                             },
                                                           {1.4375, -0.805, 2.53125}, {
                                                                                          0.805, -1.4375,
                                                                                          2.53125
                                                                                      }, {0, -1.4375, 2.53125},
                                                           {1.5, 0, 2.4}, {
                                                                              1.5, -0.84,
                                                                              2.4
                                                                          }, {0.84, -1.5, 2.4}, {0, -1.5, 2.4},
                                                           {1.75, 0, 1.875},
                                                           {1.75, -0.98, 1.875}, {0.98, -1.75, 1.875}, {
                                                                                                           0, -1.75,
                                                                                                           1.875
                                                                                                       }, {2, 0, 1.35},
                                                           {2, -1.12, 1.35}, {1.12, -2, 1.35},
                                                           {0, -2, 1.35}, {2, 0, 0.9}, {2, -1.12, 0.9}, {
                                                                                                            1.12, -2,
                                                                                                            0.9
                                                                                                        }, {0, -2, 0.9},
                                                           {-2, 0, 0.9}, {2, 0, 0.45}, {
                                                                                           2, -1.12,
                                                                                           0.45
                                                                                       }, {1.12, -2, 0.45},
                                                           {0, -2, 0.45}, {1.5, 0, 0.225},
                                                           {1.5, -0.84, 0.225}, {0.84, -1.5, 0.225}, {0, -1.5, 0.225},
                                                           {1.5, 0, 0.15}, {1.5, -0.84, 0.15}, {0.84, -1.5, 0.15}, {
                                                                                                                       0
                                                                                                                       ,
                                                                                                                       -1.5
                                                                                                                       ,
                                                                                                                       0.15
                                                                                                                   },
                                                           {-1.6, 0, 2.025}, {-1.6, -0.3, 2.025}, {
                                                                                                      -1.5,
                                                                                                      -0.3, 2.25
                                                                                                  }, {-1.5, 0, 2.25},
                                                           {-2.3, 0, 2.025}, {
                                                                                 -2.3, -0.3,
                                                                                 2.025
                                                                             }, {-2.5, -0.3, 2.25}, {-2.5, 0, 2.25}, {
                                                                                                                         -2.7
                                                                                                                         ,
                                                                                                                         0
                                                                                                                         ,
                                                                                                                         2.025
                                                                                                                     },
                                                           {-2.7, -0.3, 2.025}, {-3, -0.3, 2.25}, {
                                                                                                      -3, 0,
                                                                                                      2.25
                                                                                                  }, {-2.7, 0, 1.8},
                                                           {-2.7, -0.3, 1.8}, {-3, -0.3, 1.8},
                                                           {-3, 0, 1.8}, {-2.7, 0, 1.575}, {-2.7, -0.3, 1.575}, {
                                                                                                                    -3,
                                                                                                                    -0.3
                                                                                                                    ,
                                                                                                                    1.35
                                                                                                                },
                                                           {-3, 0, 1.35}, {-2.5, 0, 1.125}, {
                                                                                                -2.5, -0.3,
                                                                                                1.125
                                                                                            }, {-2.65, -0.3, 0.9375},
                                                           {-2.65, 0, 0.9375}, {
                                                                                   -2,
                                                                                   -0.3, 0.9
                                                                               }, {-1.9, -0.3, 0.6}, {-1.9, 0, 0.6}, {
                                                                                                                         1.7
                                                                                                                         ,
                                                                                                                         0
                                                                                                                         ,
                                                                                                                         1.425
                                                                                                                     },
                                                           {1.7, -0.66, 1.425}, {1.7, -0.66, 0.6}, {
                                                                                                       1.7, 0,
                                                                                                       0.6
                                                                                                   }, {2.6, 0, 1.425},
                                                           {2.6, -0.66, 1.425}, {
                                                                                    3.1, -0.66,
                                                                                    0.825
                                                                                }, {3.1, 0, 0.825}, {2.3, 0, 2.1},
                                                           {2.3, -0.25, 2.1},
                                                           {2.4, -0.25, 2.025}, {2.4, 0, 2.025}, {2.7, 0, 2.4}, {
                                                                                                                    2.7,
                                                                                                                    -0.25
                                                                                                                    ,
                                                                                                                    2.4
                                                                                                                },
                                                           {3.3, -0.25, 2.4}, {3.3, 0, 2.4}, {
                                                                                                 2.8, 0,
                                                                                                 2.475
                                                                                             }, {2.8, -0.25, 2.475},
                                                           {3.525, -0.25, 2.49375},
                                                           {3.525, 0, 2.49375}, {2.9, 0, 2.475}, {2.9, -0.15, 2.475},
                                                           {3.45, -0.15, 2.5125}, {3.45, 0, 2.5125}, {2.8, 0, 2.4},
                                                           {2.8, -0.15, 2.4}, {3.2, -0.15, 2.4}, {3.2, 0, 2.4}, {
                                                                                                                    0, 0
                                                                                                                    ,
                                                                                                                    3.15
                                                                                                                },
                                                           {0.8, 0, 3.15}, {0.8, -0.45, 3.15}, {
                                                                                                   0.45, -0.8,
                                                                                                   3.15
                                                                                               }, {0, -0.8, 3.15},
                                                           {0, 0, 2.85}, {1.4, 0, 2.4}, {
                                                                                            1.4,
                                                                                            -0.784, 2.4
                                                                                        }, {0.784, -1.4, 2.4},
                                                           {0, -1.4, 2.4}, {
                                                                               0.4, 0,
                                                                               2.55
                                                                           }, {0.4, -0.224, 2.55}, {0.224, -0.4, 2.55},
                                                           {
                                                               0, -0.4,
                                                               2.55
                                                           }, {1.3, 0, 2.55}, {1.3, -0.728, 2.55}, {
                                                                                                       0.728, -1.3,
                                                                                                       2.55
                                                                                                   }, {0, -1.3, 2.55},
                                                           {1.3, 0, 2.4}, {1.3, -0.728, 2.4},
                                                           {0.728, -1.3, 2.4}, {0, -1.3, 2.4}, {0, 0, 0}, {
                                                                                                              1.425,
                                                                                                              -0.798, 0
                                                                                                          },
                                                           {1.5, 0, 0.075}, {1.425, 0, 0}, {
                                                                                               0.798, -1.425,
                                                                                               0
                                                                                           }, {0, -1.5, 0.075},
                                                           {0, -1.425, 0}, {1.5, -0.84, 0.075},
                                                           {0.84, -1.5, 0.075}
                                                       };

        private static readonly float[,,] Tex = new[,,]
                                                    {
                                                        {
                                                            {0.0f, 0},
                                                            {1, 0}
                                                        },
                                                        {
                                                            {0, 1},
                                                            {1, 1}
                                                        }
                                                    };

        private static readonly IntPtr QuadObj = Glu.NewQuadric();

        public static void CoordinateSystem()
        {
            GL.Begin(BeginMode.Lines);
            GL.Color3(Color.Red); // X Axis
            GL.Vertex3(Vector3d.Zero);
            GL.Vertex3(10, 0, 0);

            GL.Color3(Color.FromArgb(0, 255, 0)); // Y Axis
            GL.Vertex3(Vector3d.Zero);
            GL.Vertex3(0, 10, 0);

            GL.Color3(Color.Blue); // Z Axis
            GL.Vertex3(Vector3d.Zero);
            GL.Vertex3(0, 0, 10);
            GL.End();
        }

        public static void WireTeapot(double scale)
        {
            Teapot(10, scale, MeshMode2.Line);
        }

        private static void Teapot(int grid, double scale, MeshMode2 type)
        {
            var p = new float[4,4,3];
            var q = new float[4,4,3];
            var r = new float[4,4,3];
            var s = new float[4,4,3];

            GL.PushAttrib(AttribMask.EnableBit | AttribMask.EvalBit);
            GL.Enable(EnableCap.AutoNormal);
            GL.Enable(EnableCap.Normalize);
            GL.Enable(EnableCap.Map2Vertex3);
            GL.Enable(EnableCap.Map2TextureCoord2);
            GL.PushMatrix();
            GL.Rotate(270.0, 1.0, 0.0, 0.0);
            GL.Scale(0.5*scale, 0.5*scale, 0.5*scale);
            GL.Translate(0.0, 0.0, -1.5);
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    for (int k = 0; k < 4; k++)
                    {
                        for (int l = 0; l < 3; l++)
                        {
                            p[j, k, l] = (float) Cpdata[Patchdata[i, j*4 + k], l];
                            q[j, k, l] = (float) Cpdata[Patchdata[i, j*4 + (3 - k)], l];
                            if (l == 1)
                                q[j, k, l] *= -1;
                            if (i < 6)
                            {
                                r[j, k, l] = (float) Cpdata[Patchdata[i, j*4 + (3 - k)], l];
                                if (l == 0)
                                    r[j, k, l] *= -1;
                                s[j, k, l] = (float) Cpdata[Patchdata[i, j*4 + k], l];
                                if (l == 0)
                                    s[j, k, l] *= -1;
                                if (l == 1)
                                    s[j, k, l] *= -1;
                            }
                        }
                    }
                }

                GL.Map2(MapTarget.Map2TextureCoord2, 0, 1, 2, 2, 0, 1, 4, 2, ref Tex[0, 0, 0]);
                GL.Map2(MapTarget.Map2Vertex3, 0, 1, 3, 4, 0, 1, 12, 4, ref p[0, 0, 0]);
                GL.MapGrid2(grid, 0.0, 1.0, grid, 0.0, 1.0);
                GL.EvalMesh2((OpenTK.Graphics.OpenGL.MeshMode2) type, 0, grid, 0, grid);
                GL.Map2(MapTarget.Map2Vertex3, 0, 1, 3, 4, 0, 1, 12, 4,
                        ref q[0, 0, 0]);
                GL.EvalMesh2((OpenTK.Graphics.OpenGL.MeshMode2) type, 0, grid, 0, grid);
                if (i < 6)
                {
                    GL.Map2(MapTarget.Map2Vertex3, 0, 1, 3, 4, 0, 1, 12, 4, ref r[0, 0, 0]);
                    GL.EvalMesh2((OpenTK.Graphics.OpenGL.MeshMode2) type, 0, grid, 0, grid);
                    GL.Map2(MapTarget.Map2Vertex3, 0, 1, 3, 4, 0, 1, 12, 4,
                            ref s[0, 0, 0]);
                    GL.EvalMesh2((OpenTK.Graphics.OpenGL.MeshMode2) type, 0, grid, 0, grid);
                }
            }
            GL.PopMatrix();
            GL.PopAttrib();
        }

        public static void WireSphere(double radius, int slices, int stacks)
        {
            Glu.QuadricDrawStyle(QuadObj, QuadricDrawStyle.Line);
            Glu.QuadricNormal(QuadObj, QuadricNormal.Smooth);
            /* If we ever changed/used the texture or orientation state
             of quadObj, we'd need to change it to the defaults here
             with gluQuadricTexture and/or gluQuadricOrientation. */
            Glu.Sphere(QuadObj, radius, slices, stacks);
        }

        public static void WireTorus(double innerRadius, double outerRadius, int nsides, int rings)
        {
            GL.PushAttrib(AttribMask.PolygonBit);
            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
            Doughnut(innerRadius, outerRadius, nsides, rings);
            GL.PopAttrib();
        }


        private static void Doughnut(double r, double outerR, int nsides, int rings)
        {
            double ringDelta = 2.0*Math.PI/rings;
            double sideDelta = 2.0*Math.PI/nsides;

            double theta = 0.0;
            double cosTheta = 1.0;
            double sinTheta = 0.0;
            for (int i = rings - 1; i >= 0; i--)
            {
                double theta1 = theta + ringDelta;
                double cosTheta1 = Math.Cos(theta1);
                double sinTheta1 = Math.Sin(theta1);
                GL.Begin(BeginMode.QuadStrip);
                double phi = 0.0;
                for (int j = nsides; j >= 0; j--)
                {
                    phi += sideDelta;
                    double cosPhi = Math.Cos(phi);
                    double sinPhi = Math.Sin(phi);
                    double dist = outerR + r*cosPhi;

                    GL.Normal3(cosTheta1*cosPhi, -sinTheta1*cosPhi, sinPhi);
                    GL.Vertex3(cosTheta1*dist, -sinTheta1*dist, r*sinPhi);
                    GL.Normal3(cosTheta*cosPhi, -sinTheta*cosPhi, sinPhi);
                    GL.Vertex3(cosTheta*dist, -sinTheta*dist, r*sinPhi);
                }
                GL.End();
                theta = theta1;
                cosTheta = cosTheta1;
                sinTheta = sinTheta1;
            }
        }

        public static void InstructionsTrab2()
        {
            const string w = "W - View moves forward";
            const string s = "S - View moves backward";
            const string a = "A - View strafes left";
            const string d = "D - View strafes Right";
            const string f = "F - View moves up";
            const string v = "V - View moves down";
            const string mouse = "Mouse - Look about the scene";
            const string leftMouse = "Left Mouse - Perform looking";

            string instruc = string.Join(Environment.NewLine, new[] {w, s, a, d, f, v, mouse, leftMouse});

            var font = new Font("Calibri", 12.0f);

            using (var printer = new TextPrinter(TextQuality.High))
            {
                printer.Begin();

                RectangleF measure = printer.Measure(instruc, font).BoundingBox;
                measure.Width = measure.Width + 5;
                measure.Location = new PointF(5, 5);

                printer.Print(instruc, font, Color.White, measure);

                printer.End();
            }
        }

        public static void SolidCube()
        {
            // cube ///////////////////////////////////////////////////////////////////////
            //    v6----- v5
            //   /|      /|
            //  v1------v0|
            //  | |     | |
            //  | |v7---|-|v4
            //  |/      |/
            //  v2------v3

            // vertex coords array
            float[] vertices = {
                                   1, 1, 1, -1, 1, 1, -1, -1, 1, 1, -1, 1, // v0-v1-v2-v3
                                   1, 1, 1, 1, -1, 1, 1, -1, -1, 1, 1, -1, // v0-v3-v4-v5
                                   1, 1, 1, 1, 1, -1, -1, 1, -1, -1, 1, 1, // v0-v5-v6-v1
                                   -1, 1, 1, -1, 1, -1, -1, -1, -1, -1, -1, 1, // v1-v6-v7-v2
                                   -1, -1, -1, 1, -1, -1, 1, -1, 1, -1, -1, 1, // v7-v4-v3-v2
                                   1, -1, -1, -1, -1, -1, -1, 1, -1, 1, 1, -1
                               }; // v4-v7-v6-v5

            // normal array
            float[] normals = {
                                  0, 0, 1, 0, 0, 1, 0, 0, 1, 0, 0, 1, // v0-v1-v2-v3
                                  1, 0, 0, 1, 0, 0, 1, 0, 0, 1, 0, 0, // v0-v3-v4-v5
                                  0, 1, 0, 0, 1, 0, 0, 1, 0, 0, 1, 0, // v0-v5-v6-v1
                                  -1, 0, 0, -1, 0, 0, -1, 0, 0, -1, 0, 0, // v1-v6-v7-v2
                                  0, -1, 0, 0, -1, 0, 0, -1, 0, 0, -1, 0, // v7-v4-v3-v2
                                  0, 0, -1, 0, 0, -1, 0, 0, -1, 0, 0, -1
                              }; // v4-v7-v6-v5

            // color array
            float[] colors = {
                                 0, 0, 1, 0, 0, 1, 0, 0, 1, 0, 0, 1,
                                 1, 0, 0, 1, 0, 0, 1, 0, 0, 1, 0, 0,
                                 0, 1, 0, 0, 1, 0, 0, 1, 0, 0, 1, 0,
                                 1, 0, 1, 1, 0, 1, 1, 0, 1, 1, 0, 1,
                                 0, 1, 1, 0, 1, 1, 0, 1, 1, 0, 1, 1,
                                 1, 1, 0, 1, 1, 0, 1, 1, 0, 1, 1, 0
                             };

            // index array of vertex array for glDrawElements()
            // Notice the indices are listed straight from beginning to end as exactly
            // same order of vertex array without hopping, because of different normals at
            // a shared vertex. For this case, glDrawArrays() and glDrawElements() have no
            // difference.
            int[] indices = {
                                0, 1, 2, 3,
                                4, 5, 6, 7,
                                8, 9, 10, 11,
                                12, 13, 14, 15,
                                16, 17, 18, 19,
                                20, 21, 22, 23
                            };

            // enable and specify pointers to vertex arrays
            GL.EnableClientState(ArrayCap.NormalArray);
            GL.EnableClientState(ArrayCap.ColorArray);
            GL.EnableClientState(ArrayCap.VertexArray);
            GL.NormalPointer(NormalPointerType.Float, 0, normals);
            GL.ColorPointer(3, ColorPointerType.Float, 0, colors);
            GL.VertexPointer(3, VertexPointerType.Float, 0, vertices);

            GL.PushMatrix();
            //GL.Translate(-2, -2, 0);                // move to bottom-left

            GL.DrawElements(BeginMode.Quads, 24, DrawElementsType.UnsignedInt, indices);

            GL.PopMatrix();

            GL.DisableClientState(ArrayCap.VertexArray); // disable vertex arrays
            GL.DisableClientState(ArrayCap.ColorArray);
            GL.DisableClientState(ArrayCap.NormalArray);
        }
    }
}
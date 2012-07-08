namespace Utilities
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.Reflection;
    using System.Text.RegularExpressions;
    using Exceptions;
    using OpenTK;
    using OpenTK.Graphics;

    public static class MeshParser
    {
        #region Const

        private const string ObjectName = "Name";
        private const string TriNumber = "TrianglesNumber";
        private const string MatCount = "MaterialCount";
        private const string RedComponent = "R";
        private const string GreenComponent = "G";
        private const string BlueComponent = "B";
        private const string Shine = "Shininess";
        private const string PosX = "PosX";
        private const string PosY = "PosY";
        private const string PosZ = "PosZ";
        private const string NorX = "NorX";
        private const string NorY = "NorY";
        private const string NorZ = "NorZ";
        private const string ColorIndex = "Index";
        private const string FaceX = "FaceX";
        private const string FaceY = "FaceY";
        private const string FaceZ = "FaceZ";
        private const RegexOptions Options = RegexOptions.IgnoreCase | RegexOptions.Compiled;

        #endregion

        #region Regex

        private static readonly Regex Shininess = new Regex(@"material[\s]*shine[\s]*(?<" + Shine + @">[0-9\.0-9]+)",
                                                            Options);

        private static readonly Regex AmbientColor =
            new Regex(@"ambient[\s]*color[\s]*(?<" + RedComponent + @">[0-9\.0-9]+)[\s]*(?<"
                      + GreenComponent + @">[0-9\.0-9]+)[\s]*(?<" + BlueComponent +
                      @">[0-9\.0-9]+)", Options);

        private static readonly Regex DiffuseColor =
            new Regex(@"diffuse[\s]*color[\s]*(?<" + RedComponent + @">[0-9\.0-9]+)[\s]*(?<"
                      + GreenComponent + @">[0-9\.0-9]+)[\s]*(?<" + BlueComponent +
                      @">[0-9\.0-9]+)", Options);

        private static readonly Regex SpecularColor =
            new Regex(@"specular[\s]*color[\s]*(?<" + RedComponent + @">[0-9\.0-9]+)[\s]*(?<"
                      + GreenComponent + @">[0-9\.0-9]+)[\s]*(?<" + BlueComponent +
                      @">[0-9\.0-9]+)", Options);

        private static readonly Regex MaterialCount =
            new Regex(@"Material[\s]*count[\s]*=[\s]*(?<" + MatCount + @">[0-9]+)",
                      Options);

        private static readonly Regex Name = new Regex(@"Object[\s]*name[\s]*=[\s]*(?<" + ObjectName + @">[^/\r\n]+)",
                                                       Options);

        private static readonly Regex TrianglesNumber =
            new Regex(@"#[\s]*triangles[\s]*=[\s]*(?<" + TriNumber + @">[0-9]+)",
                      Options);

        private static readonly Regex V0 =
            new Regex(
                @"v0[\s]*(?<" + PosX + @">[-]*[0-9\.0-9]+)[\s]*(?<" + PosY + @">[-]*[0-9\.0-9]+)[\s]*(?<" + PosZ +
                @">[-]*[0-9\.0-9]+)[\s]*(?<" + NorX + @">[-]*[0-9\.0-9]+)[\s]*(?<" + NorY + @">[-]*[0-9\.0-9]+)[\s]*(?<" +
                NorZ + @">[-]*[0-9\.0-9]+)[\s]*(?<" + ColorIndex + @">[0-9]+)", Options);

        private static readonly Regex V1 =
            new Regex(
                @"v1[\s]*(?<" + PosX + @">[-]*[0-9\.0-9]+)[\s]*(?<" + PosY + @">[-]*[0-9\.0-9]+)[\s]*(?<" + PosZ +
                @">[-]*[0-9\.0-9]+)[\s]*(?<" + NorX + @">[-]*[0-9\.0-9]+)[\s]*(?<" + NorY + @">[-]*[0-9\.0-9]+)[\s]*(?<" +
                NorZ + @">[-]*[0-9\.0-9]+)[\s]*(?<" + ColorIndex + @">[0-9]+)", Options);

        private static readonly Regex V2 =
            new Regex(
                @"v2[\s]*(?<" + PosX + @">[-]*[0-9\.0-9]+)[\s]*(?<" + PosY + @">[-]*[0-9\.0-9]+)[\s]*(?<" + PosZ +
                @">[-]*[0-9\.0-9]+)[\s]*(?<" + NorX + @">[-]*[0-9\.0-9]+)[\s]*(?<" + NorY + @">[-]*[0-9\.0-9]+)[\s]*(?<" +
                NorZ + @">[-]*[0-9\.0-9]+)[\s]*(?<" + ColorIndex + @">[0-9]+)", Options);

        private static readonly Regex FaceNormal =
            new Regex(@"face[\s]*normal[\s]*(?<" + FaceX + @">[-]*[0-9\.0-9]+)[\s]*(?<"
                      + FaceY + @">[-]*[0-9\.0-9]+)[\s]*(?<" + FaceZ +
                      @">[-]*[0-9\.0-9]+)", Options);

        private static readonly CultureInfo SpecificCultureInfoEnUs = CultureInfo.CreateSpecificCulture("en-US");

        #endregion

        #region Parsers

        public static string ParseName(string text)
        {
            Match match = Name.Match(text);

            if (match.Success)
            {
                if (match.Groups[ObjectName].Success)
                    return match.Groups[ObjectName].Value;
            }

            throw new RegexException(MethodBase.GetCurrentMethod());
        }

        public static int ParseTrianglesNumber(string text)
        {
            Match match = TrianglesNumber.Match(text);

            if (match.Success)
            {
                if (match.Groups[TriNumber].Success)
                    return int.Parse(match.Groups[TriNumber].Value);
            }

            throw new RegexException(MethodBase.GetCurrentMethod());
        }

        public static int ParseMaterialCount(string text)
        {
            Match match = MaterialCount.Match(text);

            if (match.Success)
            {
                if (match.Groups[MatCount].Success)
                    return int.Parse(match.Groups[MatCount].Value);
            }

            throw new RegexException(MethodBase.GetCurrentMethod());
        }

        public static List<Material> ParseMaterials(string text)
        {
            int count = ParseMaterialCount(text);
            var materials = new List<Material>(count);

            MatchCollection ambientMatches = AmbientColor.Matches(text);
            MatchCollection diffuseMatches = DiffuseColor.Matches(text);
            MatchCollection specularMatches = SpecularColor.Matches(text);
            Match shineMatch = Shininess.Match(text);

            for (int i = 0; i < count; i++)
            {
                var material = new Material();
                if (ambientMatches[i].Success)
                {
                    material.Ambient =
                        new Color4(float.Parse(ambientMatches[i].Groups[RedComponent].Value,
                                               SpecificCultureInfoEnUs),
                                   float.Parse(ambientMatches[i].Groups[GreenComponent].Value,
                                               SpecificCultureInfoEnUs),
                                   float.Parse(ambientMatches[i].Groups[BlueComponent].Value,
                                               SpecificCultureInfoEnUs), 1);
                }
                else
                    throw new RegexException(MethodBase.GetCurrentMethod());

                if (diffuseMatches[i].Success)
                {
                    material.Diffuse =
                        new Color4(float.Parse(diffuseMatches[i].Groups[RedComponent].Value,
                                               SpecificCultureInfoEnUs),
                                   float.Parse(diffuseMatches[i].Groups[GreenComponent].Value,
                                               SpecificCultureInfoEnUs),
                                   float.Parse(diffuseMatches[i].Groups[BlueComponent].Value,
                                               SpecificCultureInfoEnUs), 1);
                }
                else
                    throw new RegexException(MethodBase.GetCurrentMethod());

                if (specularMatches[i].Success)
                {
                    material.Specular =
                        new Color4(float.Parse(specularMatches[i].Groups[RedComponent].Value,
                                               SpecificCultureInfoEnUs),
                                   float.Parse(specularMatches[i].Groups[GreenComponent].Value,
                                               SpecificCultureInfoEnUs),
                                   float.Parse(specularMatches[i].Groups[BlueComponent].Value,
                                               SpecificCultureInfoEnUs), 1);
                }
                else
                    throw new RegexException(MethodBase.GetCurrentMethod());

                if (shineMatch.Success)
                {
                    material.Shininess = float.Parse(shineMatch.Groups[Shine].Value, SpecificCultureInfoEnUs);
                }
                else
                    throw new RegexException(MethodBase.GetCurrentMethod());

                materials.Add(material);
            }

            return materials;
        }

        public static List<Triangle> ParseTriangules(string text)
        {
            int number = ParseTrianglesNumber(text);
            var triangules = new List<Triangle>(number);

            MatchCollection v0Matches = V0.Matches(text);
            MatchCollection v1Matches = V1.Matches(text);
            MatchCollection v2Matches = V2.Matches(text);
            MatchCollection faceNormalMatch = FaceNormal.Matches(text);

            for (int i = 0; i < number; i++)
            {
                var triangle = new Triangle();
                if (v0Matches[i].Success)
                {
                    triangle.V0 =
                        new Vertex
                            {
                                Position = new Vector3(float.Parse(v0Matches[i].Groups[PosX].Value,
                                                                   SpecificCultureInfoEnUs),
                                                       float.Parse(v0Matches[i].Groups[PosY].Value,
                                                                   SpecificCultureInfoEnUs),
                                                       float.Parse(v0Matches[i].Groups[PosZ].Value,
                                                                   SpecificCultureInfoEnUs)),
                                Normal = new Vector3(float.Parse(v0Matches[i].Groups[NorX].Value,
                                                                 SpecificCultureInfoEnUs),
                                                     float.Parse(v0Matches[i].Groups[NorY].Value,
                                                                 SpecificCultureInfoEnUs),
                                                     float.Parse(v0Matches[i].Groups[NorZ].Value,
                                                                 SpecificCultureInfoEnUs)),
                                ColorIndex = int.Parse(v0Matches[i].Groups[ColorIndex].Value)
                            };
                }
                else
                    throw new RegexException(MethodBase.GetCurrentMethod());

                if (v1Matches[i].Success)
                {
                    triangle.V1 =
                        new Vertex
                            {
                                Position = new Vector3(float.Parse(v1Matches[i].Groups[PosX].Value,
                                                                   SpecificCultureInfoEnUs),
                                                       float.Parse(v1Matches[i].Groups[PosY].Value,
                                                                   SpecificCultureInfoEnUs),
                                                       float.Parse(v1Matches[i].Groups[PosZ].Value,
                                                                   SpecificCultureInfoEnUs)),
                                Normal = new Vector3(float.Parse(v1Matches[i].Groups[NorX].Value,
                                                                 SpecificCultureInfoEnUs),
                                                     float.Parse(v1Matches[i].Groups[NorY].Value,
                                                                 SpecificCultureInfoEnUs),
                                                     float.Parse(v1Matches[i].Groups[NorZ].Value,
                                                                 SpecificCultureInfoEnUs)),
                                ColorIndex = int.Parse(v1Matches[i].Groups[ColorIndex].Value)
                            };
                }
                else
                    throw new RegexException(MethodBase.GetCurrentMethod());

                if (v2Matches[i].Success)
                {
                    triangle.V2 =
                        new Vertex
                            {
                                Position = new Vector3(float.Parse(v2Matches[i].Groups[PosX].Value,
                                                                   SpecificCultureInfoEnUs),
                                                       float.Parse(v2Matches[i].Groups[PosY].Value,
                                                                   SpecificCultureInfoEnUs),
                                                       float.Parse(v2Matches[i].Groups[PosZ].Value,
                                                                   SpecificCultureInfoEnUs)),
                                Normal = new Vector3(float.Parse(v2Matches[i].Groups[NorX].Value,
                                                                 SpecificCultureInfoEnUs),
                                                     float.Parse(v2Matches[i].Groups[NorY].Value,
                                                                 SpecificCultureInfoEnUs),
                                                     float.Parse(v2Matches[i].Groups[NorZ].Value,
                                                                 SpecificCultureInfoEnUs)),
                                ColorIndex = int.Parse(v2Matches[i].Groups[ColorIndex].Value)
                            };
                }
                else
                    throw new RegexException(MethodBase.GetCurrentMethod());

                if (faceNormalMatch[i].Success)
                {
                    triangle.FaceNormal =
                        new Vector3(float.Parse(faceNormalMatch[i].Groups[FaceX].Value,
                                                SpecificCultureInfoEnUs),
                                    float.Parse(faceNormalMatch[i].Groups[FaceY].Value,
                                                SpecificCultureInfoEnUs),
                                    float.Parse(faceNormalMatch[i].Groups[FaceZ].Value,
                                                SpecificCultureInfoEnUs));
                }
                else
                    throw new RegexException(MethodBase.GetCurrentMethod());

                triangules.Add(triangle);
            }

            return triangules;
        }

        #endregion
    }
}
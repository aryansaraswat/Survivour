﻿//////////////////////////////////////////////////////
// MicroSplat
// Copyright (c) Jason Booth
//////////////////////////////////////////////////////

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// layout
// 0 perTex UV Scale and Offset
// 1 PerTex Tint and interpolation contrast (a)
// 2 Normal Strength, Smoothness, AO, Metallic
// 3 Brightness, Contrast, porosity, foam
// 4 DetailNoiseStrength, distance Noise Strength, Distance Resample, DisplaceMip
// 5 geoTex, tint strength, normal strength, SmoothAOMetal strength
// 6 displace, bias, offset, global emis strength
// 7 Noise 0, Noise 1, Noise 2, Wind Particulate Strength
// 8 Snow (R), Glitter (G), GeoHeightFilter(B), (A) GeoHeightFilterStrength 
// 9 Triplanar, trplanar contrast, stochastic enabled, (A) Saturation
// 10 Texture Cluster Contrast, boost, Height Offset, Height Contrast
// 11 Advanced Detail UV Scale/Offset
// 12 Advanced Detail (G)Normal Blend, (B)Tex Overlay (A) MeshOverlayNormalBlend
// 13 Advanced Detail (R)Contrast, (G) AngleContrast, (B)HeightConttast, (A) Distance Resample UV scale
// 14 AntiTileArray (R)Normal Str, (G) Detail Strength, (B) Distance Strength (A) DisplaceShaping
// 15 Reserved for initialization marking
// 16 UV Rotation, Triplanar rot, triplanar rot, (A) Open
// 17
// 18

// because unity's HDR import pipeline is broke (assumes gamma, so breaks data in textures)
public class MicroSplatPropData : ScriptableObject 
{
   const int sMaxTextures = 32;
   const int sMaxAttributes = 32;
   [HideInInspector]
   public Color[] values = new Color[sMaxTextures* sMaxAttributes];

   Texture2D tex;

   [HideInInspector]
   public AnimationCurve geoCurve = AnimationCurve.Linear(0, 0.0f, 0, 0.0f);
   Texture2D geoTex;

   [HideInInspector]
   public AnimationCurve geoSlopeFilter = AnimationCurve.Linear(0, 0.2f, 0.4f, 1.0f);
   Texture2D geoSlopeTex;

   [HideInInspector]
   public AnimationCurve globalSlopeFilter = AnimationCurve.Linear(0, 0.2f, 0.4f, 1.0f);
   Texture2D globalSlopeTex;

   void RevisionData()
   {
      // revision from 16 to 32 max textures
      if (values.Length == (16 * 16))
      {
         Color[] c = new Color[sMaxTextures * sMaxAttributes];
         for (int x = 0; x < 16; ++x)
         {
            for (int y = 0; y < 16; ++y)
            {
               c[y * sMaxTextures + x] = values[y * sMaxAttributes + x];
            }
         }
         values = c;
         #if UNITY_EDITOR
         UnityEditor.EditorUtility.SetDirty(this);
         #endif
      }
      else if (values.Length == (32 * 16))
      {
         Color [] c = new Color [sMaxTextures * sMaxAttributes];
         for (int x = 0; x < 32; ++x)
         {
            for (int y = 0; y < 16; ++y)
            {
               c [y * sMaxTextures + x] = values [y * sMaxAttributes + x];
            }
         }
         values = c;
#if UNITY_EDITOR
         UnityEditor.EditorUtility.SetDirty (this);
#endif
      }
   }

   public Color GetValue(int x, int y)
   {
      RevisionData();
      return values[y * sMaxTextures + x];
   }

   public void SetValue(int x, int y, Color c)
   {
      RevisionData();
      #if UNITY_EDITOR
      UnityEditor.Undo.RecordObject(this, "Changed Value");
      #endif

      values[y * sMaxTextures + x] = c;

      #if UNITY_EDITOR
      UnityEditor.EditorUtility.SetDirty(this);
      #endif
   }

   public void SetValue(int x, int y, int channel, float value)
   {
      RevisionData();
      #if UNITY_EDITOR
      UnityEditor.Undo.RecordObject(this, "Changed Value");
      #endif
      int index = y * sMaxTextures + x;
      Color c = values[index];
      c[channel] = value;
      values[index] = c;

      #if UNITY_EDITOR
      UnityEditor.EditorUtility.SetDirty(this);
      #endif
   }

   public Texture2D GetTexture()
   {
      RevisionData();
      if (tex == null)
      {
         if (SystemInfo.SupportsTextureFormat (TextureFormat.RGBAFloat))
         {
            tex = new Texture2D(sMaxTextures, sMaxAttributes, TextureFormat.RGBAFloat, false, true);
         }
         else if (SystemInfo.SupportsTextureFormat (TextureFormat.RGBAHalf))
         {
            tex = new Texture2D(sMaxTextures, sMaxAttributes, TextureFormat.RGBAHalf, false, true);
         }
         else
         {
            Debug.LogError ("Could not create RGBAFloat or RGBAHalf format textures, per texture properties will be clamped to 0-1 range, which will break things");
            tex = new Texture2D (sMaxTextures, sMaxAttributes, TextureFormat.RGBA32, false, true);
         }
         tex.hideFlags = HideFlags.HideAndDontSave;
         tex.wrapMode = TextureWrapMode.Clamp;
         tex.filterMode = FilterMode.Point;

      }
      tex.SetPixels(values);
      tex.Apply();
      return tex;
   }

   public Texture2D GetGeoCurve()
   {
      if (geoTex == null)
      {
         geoTex = new Texture2D(256, 1, TextureFormat.RHalf, false, true);
         geoTex.hideFlags = HideFlags.HideAndDontSave;
      }
      for (int i = 0; i < 256; ++i)
      {
         float v = geoCurve.Evaluate((float)i / 255.0f);
         geoTex.SetPixel(i, 0, new Color(v, v, v, v));
      }
      geoTex.Apply();
      return geoTex;
   }

   public Texture2D GetGeoSlopeFilter()
   {
      if (geoSlopeTex == null)
      {
         geoSlopeTex = new Texture2D(256, 1, TextureFormat.Alpha8, false, true);
         geoSlopeTex.hideFlags = HideFlags.HideAndDontSave;
      }
      for (int i = 0; i < 256; ++i)
      {
         float v = geoSlopeFilter.Evaluate((float)i / 255.0f);
         geoSlopeTex.SetPixel(i, 0, new Color(v, v, v, v));
      }
      geoSlopeTex.Apply();
      return geoSlopeTex;
   }

   public Texture2D GetGlobalSlopeFilter()
   {
      if (globalSlopeTex == null)
      {
         globalSlopeTex = new Texture2D(256, 1, TextureFormat.Alpha8, false, true);
         globalSlopeTex.hideFlags = HideFlags.HideAndDontSave;
      }
      for (int i = 0; i < 256; ++i)
      {
         float v = globalSlopeFilter.Evaluate((float)i / 255.0f);
         globalSlopeTex.SetPixel(i, 0, new Color(v, v, v, v));
      }
      globalSlopeTex.Apply();
      return globalSlopeTex;
   }
}


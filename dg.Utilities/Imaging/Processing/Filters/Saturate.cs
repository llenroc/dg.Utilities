﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing.Imaging;

namespace dg.Utilities.Imaging.Filters
{
    public class Saturate : IImageFilter
    {
        public enum Mode
        {
            Natural = 0,
            NaturalNTSC = 1,
            Accurate = 2
        }
        public class Amount
        {
            private float _Value = 1f;
            public Amount(float amount)
            {
                Value = amount;
            }
            public float Value
            {
                get { return _Value; }
                set { _Value = value; }
            }
        }

        public ImageFilterError ProcessImage(
            DirectAccessBitmap bmp,
            params object[] args)
        {
            Mode mode = Mode.Natural;
            Amount amount = new Amount(1f);
            foreach (object arg in args)
            {
                if (arg is Mode)
                {
                    mode = (Mode)arg;
                }
                else if (arg is Amount)
                {
                    amount = (Amount)arg;
                }
            }

            switch (bmp.Bitmap.PixelFormat)
            {
                case PixelFormat.Format24bppRgb:
                    return ProcessImage24rgb(bmp, mode, amount);
                case PixelFormat.Format32bppRgb:
                    return ProcessImage32rgb(bmp, mode, amount);
                case PixelFormat.Format32bppArgb:
                    return ProcessImage32argb(bmp, mode, amount);
                case PixelFormat.Format32bppPArgb:
                    return ProcessImage32pargb(bmp, mode, amount);
                default:
                    return ImageFilterError.IncompatiblePixelFormat;
            }
        }
        public ImageFilterError ProcessImage24rgb(DirectAccessBitmap bmp, Mode mode, Amount amount)
        {
            if (amount == null) return ImageFilterError.MissingArgument;

            int cx = bmp.Width;
            int cy = bmp.Height;
            int endX = cx + bmp.StartX;
            int endY = cy + bmp.StartY;
            byte[] data = bmp.Bits;
            int stride = bmp.Stride;
            int pos1, pos2;
            int x, y;

            float rL = 0, gL = 0, bL = 0;
            if (mode == Mode.NaturalNTSC)
            {
                rL = ImagingUtility.RedLuminosityNTSC;
                gL = ImagingUtility.GreenLuminosityNTSC;
                bL = ImagingUtility.BlueLuminosityNTSC;
            }
            else if (mode == Mode.Natural)
            {
                rL = ImagingUtility.RedLuminosity;
                gL = ImagingUtility.GreenLuminosity;
                bL = ImagingUtility.BlueLuminosity;
            }
            else
            {
                rL = 0.334f;
                gL = 0.333f;
                bL = 0.333f;
            }

            float M1_1, M1_2, M1_3, M2_1, M2_2, M2_3, M3_1, M3_2, M3_3;
            float v;
            M1_1 = (1.0f - amount.Value) * rL + amount.Value;
            M1_2 = (1.0f - amount.Value) * rL;
            M1_3 = (1.0f - amount.Value) * rL;
            M2_1 = (1.0f - amount.Value) * gL;
            M2_2 = (1.0f - amount.Value) * gL + amount.Value;
            M2_3 = (1.0f - amount.Value) * gL;
            M3_1 = (1.0f - amount.Value) * bL;
            M3_2 = (1.0f - amount.Value) * bL;
            M3_3 = (1.0f - amount.Value) * bL + amount.Value;

            byte r, g, b;
            for (y = bmp.StartY; y < endY; y++)
            {
                pos1 = stride * y;
                for (x = bmp.StartX; x < endX; x++)
                {
                    pos2 = pos1 + x * 3;

                    r = data[pos2 + 2];
                    g = data[pos2 + 1];
                    b = data[pos2];

                    v = (M1_1 * r + M2_1 * g + M3_1 * b);
                    if (v > 255) v = 255; else if (v < 0) v = 0;
                    data[pos2 + 2] = (byte)v;
                    v = (M1_2 * r + M2_2 * g + M3_2 * b);
                    if (v > 255) v = 255; else if (v < 0) v = 0;
                    data[pos2 + 1] = (byte)v;
                    v = (M1_3 * r + M2_3 * g + M3_3 * b);
                    if (v > 255) v = 255; else if (v < 0) v = 0;
                    data[pos2] = (byte)v;
                }
            }

            return ImageFilterError.OK;
        }
        public ImageFilterError ProcessImage32rgb(DirectAccessBitmap bmp, Mode mode, Amount amount)
        {
            if (amount == null) return ImageFilterError.MissingArgument;

            int cx = bmp.Width;
            int cy = bmp.Height;
            int endX = cx + bmp.StartX;
            int endY = cy + bmp.StartY;
            byte[] data = bmp.Bits;
            int stride = bmp.Stride;
            int pos1, pos2;
            int x, y;

            float rL = 0, gL = 0, bL = 0;
            if (mode == Mode.NaturalNTSC)
            {
                rL = ImagingUtility.RedLuminosityNTSC;
                gL = ImagingUtility.GreenLuminosityNTSC;
                bL = ImagingUtility.BlueLuminosityNTSC;
            }
            else if (mode == Mode.Natural)
            {
                rL = ImagingUtility.RedLuminosity;
                gL = ImagingUtility.GreenLuminosity;
                bL = ImagingUtility.BlueLuminosity;
            }
            else
            {
                rL = 0.334f;
                gL = 0.333f;
                bL = 0.333f;
            }

            float M1_1, M1_2, M1_3, M2_1, M2_2, M2_3, M3_1, M3_2, M3_3;
            float v;
            M1_1 = (1.0f - amount.Value) * rL + amount.Value;
            M1_2 = (1.0f - amount.Value) * rL;
            M1_3 = (1.0f - amount.Value) * rL;
            M2_1 = (1.0f - amount.Value) * gL;
            M2_2 = (1.0f - amount.Value) * gL + amount.Value;
            M2_3 = (1.0f - amount.Value) * gL;
            M3_1 = (1.0f - amount.Value) * bL;
            M3_2 = (1.0f - amount.Value) * bL;
            M3_3 = (1.0f - amount.Value) * bL + amount.Value;

            float r, g, b;
            for (y = bmp.StartY; y < endY; y++)
            {
                pos1 = stride * y;
                for (x = bmp.StartX; x < endX; x++)
                {
                    pos2 = pos1 + x * 4;

                    r = data[pos2 + 2];
                    g = data[pos2 + 1];
                    b = data[pos2];

                    v = (M1_1 * r + M2_1 * g + M3_1 * b);
                    if (v > 255) v = 255; else if (v < 0) v = 0;
                    data[pos2 + 2] = (byte)v;
                    v = (M1_2 * r + M2_2 * g + M3_2 * b);
                    if (v > 255) v = 255; else if (v < 0) v = 0;
                    data[pos2 + 1] = (byte)v;
                    v = (M1_3 * r + M2_3 * g + M3_3 * b);
                    if (v > 255) v = 255; else if (v < 0) v = 0;
                    data[pos2] = (byte)v;
                }
            }

            return ImageFilterError.OK;
        }
        public ImageFilterError ProcessImage32argb(DirectAccessBitmap bmp, Mode mode, Amount amount)
        {
            return ProcessImage32rgb(bmp, mode, amount);
        }
        public ImageFilterError ProcessImage32pargb(DirectAccessBitmap bmp, Mode mode, Amount amount)
        {
            if (amount == null) return ImageFilterError.MissingArgument;

            int cx = bmp.Width;
            int cy = bmp.Height;
            int endX = cx + bmp.StartX;
            int endY = cy + bmp.StartY;
            byte[] data = bmp.Bits;
            int stride = bmp.Stride;
            int pos1, pos2;
            int x, y;
            float preAlpha;

            float rL = 0, gL = 0, bL = 0;
            if (mode == Mode.NaturalNTSC)
            {
                rL = ImagingUtility.RedLuminosityNTSC;
                gL = ImagingUtility.GreenLuminosityNTSC;
                bL = ImagingUtility.BlueLuminosityNTSC;
            }
            else if (mode == Mode.Natural)
            {
                rL = ImagingUtility.RedLuminosity;
                gL = ImagingUtility.GreenLuminosity;
                bL = ImagingUtility.BlueLuminosity;
            }
            else
            {
                rL = 0.334f;
                gL = 0.333f;
                bL = 0.333f;
            }

            float M1_1, M1_2, M1_3, M2_1, M2_2, M2_3, M3_1, M3_2, M3_3;
            float v;
            M1_1 = (1.0f - amount.Value) * rL + amount.Value;
            M1_2 = (1.0f - amount.Value) * rL;
            M1_3 = (1.0f - amount.Value) * rL;
            M2_1 = (1.0f - amount.Value) * gL;
            M2_2 = (1.0f - amount.Value) * gL + amount.Value;
            M2_3 = (1.0f - amount.Value) * gL;
            M3_1 = (1.0f - amount.Value) * bL;
            M3_2 = (1.0f - amount.Value) * bL;
            M3_3 = (1.0f - amount.Value) * bL + amount.Value;

            float r, g, b;
            for (y = bmp.StartY; y < endY; y++)
            {
                pos1 = stride * y;
                for (x = bmp.StartX; x < endX; x++)
                {
                    pos2 = pos1 + x * 4;

                    preAlpha = (float)data[pos2 + 3];
                    if (preAlpha > 0) preAlpha = preAlpha / 255f;
                    
                    r = data[pos2 + 2] / preAlpha;
                    g = data[pos2 + 1] / preAlpha;
                    b = data[pos2] / preAlpha;

                    v = (M1_1 * r + M2_1 * g + M3_1 * b);
                    if (v > 255) v = 255; else if (v < 0) v = 0;
                    data[pos2 + 2] = (byte)(v * preAlpha);
                    v = (M1_2 * r + M2_2 * g + M3_2 * b);
                    if (v > 255) v = 255; else if (v < 0) v = 0;
                    data[pos2 + 1] = (byte)(v * preAlpha);
                    v = (M1_3 * r + M2_3 * g + M3_3 * b);
                    if (v > 255) v = 255; else if (v < 0) v = 0;
                    data[pos2] = (byte)(v * preAlpha);
                }
            }

            return ImageFilterError.OK;
        }
    }
}
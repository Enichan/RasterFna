using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Point = Microsoft.Xna.Framework.Point;
using static SDL2.SDL;

namespace RasterFna {
    internal static class Ellipse {
        [ThreadStatic]
        private static List<Point> ovalOffsets;

        public static void DrawEllipse(IntPtr renderer, int x, int y, int radx, int rady, bool fill = false) {
            if (radx == rady) {
                // hey, this is a circle! >:(
                Circle.DrawCircle(renderer, x, y, radx, fill);
            }
            else {
                if (fill) {
                    DrawEllipse(new OvalSegmentFill(), renderer, x, y, radx, rady, fill);
                }
                else {
                    DrawEllipse(new OvalSegmentEmpty(), renderer, x, y, radx, rady, fill);
                }
            }
        }

        private static void DrawEllipse<TSegment>(TSegment seg, IntPtr renderer, int x, int y, int radx, int rady, bool fill = false) where TSegment : IOvalSegment {
            radx = Math.Abs(radx);
            rady = Math.Abs(rady);

            var xDiameter = radx * 2 + 1;
            var yDiameter = rady * 2 + 1;

            if (xDiameter == 0 || yDiameter == 0) {
                return;
            }

            long xx2 = 2 * radx * radx;
            long yy2 = 2 * rady * rady;

            long xOffset = radx;
            long yOffset = 0;

            long dx = rady * rady * (1 - 2 * radx);
            long dy = radx * radx;
            long p = 0;

            long sx = yy2 * radx;
            long sy = 0;

            var points = ovalOffsets ?? (ovalOffsets = new List<Point>());
            points.Clear();

            while (sx >= sy) {
                points.Add(new Point((int)xOffset, (int)yOffset));

                yOffset++;
                sy += xx2;
                p += dy;
                dy += xx2;

                if (2 * p + dx > 0) {
                    xOffset--;
                    sx -= yy2;
                    p += dx;
                    dx += yy2;
                }
            }

            xOffset = 0;
            yOffset = rady;

            dx = rady * rady;
            dy = radx * radx * (1 - 2 * rady);
            p = 0;

            sx = 0;
            sy = xx2 * rady;

            while (sx <= sy) {
                points.Add(new Point((int)xOffset, (int)yOffset));

                xOffset++;
                sx += yy2;
                p += dx;
                dx += yy2;

                if (2 * p + dy > 0) {
                    yOffset--;
                    sy -= xx2;
                    p += dy;
                    dy += xx2;
                }
            }

            PlotEllipse(seg, renderer, x, y, points);
        }

        private static void PlotEllipse<TSegment>(TSegment seg, IntPtr renderer, int x, int y, List<Point> points) where TSegment : IOvalSegment {
            for (int i = 0; i < points.Count; i++) {
                var offset = points[i];

                var x0 = x - offset.X;
                var x1 = x + offset.X;
                var y0 = y - offset.Y;
                var y1 = y + offset.Y;

                seg.Plot(renderer, x0, y0, x1);
                seg.Plot(renderer, x0, y1, x1);
            }
        }

        // this mess below is just to make our actual ellipse plotting code cleaner
        // because without it we either have an "if (fill)" branch in the inner loop
        // (which is slow) or be forced to duplicate code by putting it outside the
        // inner loop (unclean code), and these structs let us generify the actual
        // segment plotting code without using the performance penalty of delegates
        private interface IOvalSegment { void Plot(IntPtr renderer, int x0, int y, int x1); }

        private struct OvalSegmentFill : IOvalSegment {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Plot(IntPtr renderer, int x0, int y, int x1) {
                SDL_RenderDrawLine(renderer, x0, y, x1, y);
            }
        }

        private struct OvalSegmentEmpty : IOvalSegment {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Plot(IntPtr renderer, int x0, int y, int x1) {
                SDL_RenderDrawPoint(renderer, x0, y);
                SDL_RenderDrawPoint(renderer, x1, y);
            }
        }
    }
}

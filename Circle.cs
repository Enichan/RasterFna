using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Point = Microsoft.Xna.Framework.Point;
using static SDL2.SDL;

namespace RasterFna {
    internal static class Circle {
        [ThreadStatic]
        private static List<Point> circOffsets;

        public static void DrawCircle(IntPtr renderer, int x, int y, int radius, bool fill = false) {
            if (fill) {
                DrawCircle(new CircSegmentFill(), renderer, x, y, radius, fill);
            }
            else {
                DrawCircle(new CircSegmentEmpty(), renderer, x, y, radius, fill);
            }
        }

        private static void DrawCircle<TSegment>(TSegment seg, IntPtr renderer, int x, int y, int radius, bool fill = false) where TSegment : ICircSegment {
            radius = Math.Abs(radius);

            var diameter = radius * 2 + 1;
            if (diameter == 0) {
                return;
            }

            var xOffset = 0;
            var yOffset = radius;

            var p = 1 - radius;

            var points = circOffsets ?? (circOffsets = new List<Point>());
            points.Clear();

            while (xOffset <= yOffset) {
                points.Add(new Point(xOffset, yOffset));

                if (p < 0) {
                    p = p + 2 * xOffset + 3;
                }
                else {
                    p = p + 2 * (xOffset - yOffset) + 5;
                    yOffset--;
                }

                xOffset++;
            }

            PlotCircle(seg, renderer, x, y, points);
        }

        private static void PlotCircle<TSegment>(TSegment seg, IntPtr renderer, int x, int y, List<Point> points) where TSegment : ICircSegment {
            for (int i = 0; i < points.Count; i++) {
                var offset = points[i];

                var x0 = x - offset.X;
                var x1 = x + offset.X;
                var x2 = x - offset.Y;
                var x3 = x + offset.Y;
                var y0 = y - offset.Y;
                var y1 = y + offset.Y;
                var y2 = y - offset.X;
                var y3 = y + offset.X;

                seg.Plot(renderer, x0, y0, x1);
                seg.Plot(renderer, x0, y1, x1);
                seg.Plot(renderer, x2, y2, x3);
                seg.Plot(renderer, x2, y3, x3);
            }
        }

        // this mess below is just to make our actual circle plotting code cleaner
        // because without it we either have an "if (fill)" branch in the inner loop
        // (which is slow) or be forced to duplicate code by putting it outside the
        // inner loop (unclean code), and these structs let us generify the actual
        // segment plotting code without using the performance penalty of delegates
        private interface ICircSegment { void Plot(IntPtr renderer, int x0, int y, int x1); }

        private struct CircSegmentFill : ICircSegment {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Plot(IntPtr renderer, int x0, int y, int x1) {
                SDL_RenderDrawLine(renderer, x0, y, x1, y);
            }
        }

        private struct CircSegmentEmpty : ICircSegment {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Plot(IntPtr renderer, int x0, int y, int x1) {
                SDL_RenderDrawPoint(renderer, x0, y);
                SDL_RenderDrawPoint(renderer, x1, y);
            }
        }
    }
}

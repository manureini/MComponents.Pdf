using MComponents.MForm;
using Microsoft.Extensions.Localization;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MComponents.Pdf.Helper.PropertyField
{
    internal class PropertyFieldRenderHelper
    {
        protected SKCanvas mCanvas;
        internal float mX;
        internal float mY;
        protected IStringLocalizer mL;

        public PropertyFieldRenderHelper(SKCanvas pCanvas, float pX, float pY, IStringLocalizer pL)
        {
            mCanvas = pCanvas;
            mX = pX;
            mY = pY;
            mL = pL;
        }

        public void Render(IMPropertyField pPropertyField, object pModel)
        {
            var propInfo = ReflectionHelper.GetIMPropertyInfo(pModel.GetType(), pPropertyField.Property, pPropertyField.PropertyType);

            var paint = RenderStyles.GetDefaultPaint();

            var propname = propInfo.GetDisplayName(mL);

            mCanvas.DrawText(propname, mX, mY, paint);

            var textBounds = new SKRect();
            paint.MeasureText(propname, ref textBounds);

            mY += textBounds.Size.Height;

            var value = propInfo.GetValue(pModel);

            string str = value.ToString(); //todo formatter

            mCanvas.DrawText(str, mX, mY, paint);

            mY += textBounds.Size.Height;
        }
    }
}

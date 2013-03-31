using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Text;
using MetroFramework.Controls;

namespace MetroFramework.Design
{
    internal class MetroTabPageCollectionEditor : CollectionEditor
    {
        protected override CollectionForm CreateCollectionForm()
        {
            var baseForm = base.CreateCollectionForm();
            baseForm.Text = "MetroTabPage Collection Editor";
            return baseForm;
        }

        public MetroTabPageCollectionEditor(Type type)
            : base(type)
        { }

        protected override Type CreateCollectionItemType()
        {
            return typeof(MetroTabPage);
        }

        protected override Type[] CreateNewItemTypes()
        {
            return new[] { typeof(MetroTabPage) };
        }
    }
}

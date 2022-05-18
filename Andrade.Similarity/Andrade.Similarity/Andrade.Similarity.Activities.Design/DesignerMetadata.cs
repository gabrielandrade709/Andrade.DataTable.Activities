using System.Activities.Presentation.Metadata;
using System.ComponentModel;
using System.ComponentModel.Design;
using Andrade.Similarity.Activities.Design.Designers;
using Andrade.Similarity.Activities.Design.Properties;

namespace Andrade.Similarity.Activities.Design
{
    public class DesignerMetadata : IRegisterMetadata
    {
        public void Register()
        {
            var builder = new AttributeTableBuilder();
            builder.ValidateTable();

            var categoryAttribute = new CategoryAttribute($"{Resources.Category}");

            builder.AddCustomAttributes(typeof(SimilarityBetweenTermsInADataTable), categoryAttribute);
            builder.AddCustomAttributes(typeof(SimilarityBetweenTermsInADataTable), new DesignerAttribute(typeof(SimilarityBetweenTermsInADataTableDesigner)));
            builder.AddCustomAttributes(typeof(SimilarityBetweenTermsInADataTable), new HelpKeywordAttribute(""));


            MetadataStore.AddAttributeTable(builder.CreateTable());
        }
    }
}

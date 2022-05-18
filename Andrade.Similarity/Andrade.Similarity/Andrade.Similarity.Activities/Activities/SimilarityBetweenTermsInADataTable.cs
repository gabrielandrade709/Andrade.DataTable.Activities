using System;
using System.Activities;
using System.Threading;
using System.Threading.Tasks;
using System.Data;
using System.Linq;
using System.Collections.Generic;
using Andrade.Similarity.Activities.Properties;
using UiPath.Shared.Activities;
using UiPath.Shared.Activities.Localization;

namespace Andrade.Similarity.Activities
{
    [LocalizedDisplayName(nameof(Resources.SimilarityBetweenTermsInADataTable_DisplayName))]
    [LocalizedDescription(nameof(Resources.SimilarityBetweenTermsInADataTable_Description))]
    public class SimilarityBetweenTermsInADataTable : ContinuableAsyncCodeActivity
    {
        #region Properties

        /// <summary>
        /// If set, continue executing the remaining activities even if the current activity has failed.
        /// </summary>
        [LocalizedCategory(nameof(Resources.Common_Category))]
        [LocalizedDisplayName(nameof(Resources.ContinueOnError_DisplayName))]
        [LocalizedDescription(nameof(Resources.ContinueOnError_Description))]
        public override InArgument<bool> ContinueOnError { get; set; }

        [LocalizedDisplayName(nameof(Resources.SimilarityBetweenTermsInADataTable_TermToBeFound_DisplayName))]
        [LocalizedDescription(nameof(Resources.SimilarityBetweenTermsInADataTable_TermToBeFound_Description))]
        [LocalizedCategory(nameof(Resources.Input_Category))]
        public InArgument<string> TermToBeFound { get; set; }

        [LocalizedDisplayName(nameof(Resources.SimilarityBetweenTermsInADataTable_DataTable_DisplayName))]
        [LocalizedDescription(nameof(Resources.SimilarityBetweenTermsInADataTable_DataTable_Description))]
        [LocalizedCategory(nameof(Resources.Input_Category))]
        public InArgument<DataTable> DataTable { get; set; }

        [LocalizedDisplayName(nameof(Resources.SimilarityBetweenTermsInADataTable_SimilarityToBeFound_DisplayName))]
        [LocalizedDescription(nameof(Resources.SimilarityBetweenTermsInADataTable_SimilarityToBeFound_Description))]
        [LocalizedCategory(nameof(Resources.Input_Category))]
        public InArgument<int> SimilarityToBeFound { get; set; }

        [LocalizedDisplayName(nameof(Resources.SimilarityBetweenTermsInADataTable_LookForTheBiggestSimilarity_DisplayName))]
        [LocalizedDescription(nameof(Resources.SimilarityBetweenTermsInADataTable_LookForTheBiggestSimilarity_Description))]
        [LocalizedCategory(nameof(Resources.Input_Category))]
        public InArgument<bool> LookForTheBiggestSimilarity { get; set; }

        [LocalizedDisplayName(nameof(Resources.SimilarityBetweenTermsInADataTable_ColumnName_DisplayName))]
        [LocalizedDescription(nameof(Resources.SimilarityBetweenTermsInADataTable_ColumnName_Description))]
        [LocalizedCategory(nameof(Resources.Input_Category))]
        public InArgument<string> ColumnName { get; set; }

        [LocalizedDisplayName(nameof(Resources.SimilarityBetweenTermsInADataTable_FoundTerm_DisplayName))]
        [LocalizedDescription(nameof(Resources.SimilarityBetweenTermsInADataTable_FoundTerm_Description))]
        [LocalizedCategory(nameof(Resources.Output_Category))]
        public OutArgument<string> FoundTerm { get; set; }

        [LocalizedDisplayName(nameof(Resources.SimilarityBetweenTermsInADataTable_FoundSimilarity_DisplayName))]
        [LocalizedDescription(nameof(Resources.SimilarityBetweenTermsInADataTable_FoundSimilarity_Description))]
        [LocalizedCategory(nameof(Resources.Output_Category))]
        public OutArgument<double> FoundSimilarity { get; set; }

        #endregion


        #region Constructors

        public SimilarityBetweenTermsInADataTable()
        {
        }

        #endregion


        #region Protected Methods

        protected override void CacheMetadata(CodeActivityMetadata metadata)
        {
            if (TermToBeFound == null) metadata.AddValidationError(string.Format(Resources.ValidationValue_Error, nameof(TermToBeFound)));
            if (DataTable == null) metadata.AddValidationError(string.Format(Resources.ValidationValue_Error, nameof(DataTable)));
            if (SimilarityToBeFound == null) metadata.AddValidationError(string.Format(Resources.ValidationValue_Error, nameof(SimilarityToBeFound)));
            if (LookForTheBiggestSimilarity == null) metadata.AddValidationError(string.Format(Resources.ValidationValue_Error, nameof(LookForTheBiggestSimilarity)));
            if (ColumnName == null) metadata.AddValidationError(string.Format(Resources.ValidationValue_Error, nameof(ColumnName)));

            base.CacheMetadata(metadata);
        }

        protected override async Task<Action<AsyncCodeActivityContext>> ExecuteAsync(AsyncCodeActivityContext context, CancellationToken cancellationToken)
        {
            // Inputs
            var termToBeFound = TermToBeFound.Get(context);
            var datatable = DataTable.Get(context);
            var similarityToBeFound = SimilarityToBeFound.Get(context);
            var lookForTheBiggestSimilarity = LookForTheBiggestSimilarity.Get(context);
            var columnName = ColumnName.Get(context);

            List<String> splitString, splitStringDataTable = new List<string>();
            splitString = termToBeFound.ToLower().Split(Convert.ToChar(" ")).ToList();
            Double biggestSimilarityFound = 0.0, similarity = 0.0;
            String biggestTermSimilarityFound = "";
            int total = 0, count = 0;

            if (lookForTheBiggestSimilarity)
            {
                Console.WriteLine("Searching for a term that has the biggest similarity with '" + termToBeFound + "'.");
            }
            else
            {
                Console.WriteLine("Searching for a term that has a similarity greater than or equal to " + similarityToBeFound.ToString() + "% with the term '" + termToBeFound + "'.");
            }

            foreach (DataRow row in datatable.Rows)
            {
                splitStringDataTable = row[columnName].ToString().ToLower().Split(Convert.ToChar(" ")).ToList();
                total = splitString.Count + splitStringDataTable.Count;
                count = 0;

                foreach (String str in splitString)
                {
                    if (splitStringDataTable.Contains(str))
                    {
                        count = count + 1;
                        splitStringDataTable.Remove(str);
                    }
                }

                similarity = (100 * count * 2) / total;

                if (lookForTheBiggestSimilarity)
                {
                    if (similarity > biggestSimilarityFound)
                    {
                        biggestSimilarityFound = similarity;
                        biggestTermSimilarityFound = row[columnName].ToString();
                    }
                }
                else if (similarity >= similarityToBeFound)
                {
                    biggestSimilarityFound = similarity;
                    biggestTermSimilarityFound = row[columnName].ToString();

                    Console.WriteLine("The term '" + biggestTermSimilarityFound + " was found, which has a similarity of " + biggestSimilarityFound.ToString() + "%.");

                    break;
                }
            }

            if (string.IsNullOrEmpty(biggestTermSimilarityFound) && biggestSimilarityFound == 0.0)
            {
                Console.WriteLine("No one term greater than or equal to " + similarityToBeFound.ToString() + "% was found.");
            }

            // Outputs
            return (ctx) => {
                FoundTerm.Set(ctx, biggestTermSimilarityFound);
                FoundSimilarity.Set(ctx, biggestSimilarityFound);
            };
        }

        #endregion
    }
}



namespace MON.Services.Utils
{
    using MON.Models.DocManagement;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class DocManagementUtils
    {
        private static IEnumerable<DocManagementApplicationReportDocumentModel> ProcessSingleDocument(DocManagementApplicationBasicDocumentModel doc)
        {
            // Group delivered items by series
            var seriesGroups = doc.DeliveredItems
                .GroupBy(item => new
                {
                    Series = DocManagementUtils.GetSeriesForDocument(doc, item),
                    doc.BasicDocumentId,
                    doc.BasicDocumentName
                })
                .Where(g => g.Key.Series != null)
                .ToList();

            var results = new List<DocManagementApplicationReportDocumentModel>();

            foreach (var group in seriesGroups)
            {
                // Get all document numbers for this series
                var docNumbers = group
                    .Where(item => !string.IsNullOrEmpty(item.DocNumber))
                    .Select(item => item.DocNumber)
                    .ToList();

                // Create number ranges
                var ranges = DocManagementUtils.CreateNumberRanges(docNumbers);

                if (ranges.Any())
                {
                    // Create one model for each range
                    foreach (var range in ranges)
                    {
                        results.Add(new DocManagementApplicationReportDocumentModel
                        {
                            BasicDocumentId = group.Key.BasicDocumentId,
                            nomenclatureNumber = group.Key.BasicDocumentName.Split(" ", StringSplitOptions.RemoveEmptyEntries).FirstOrDefault() ?? "",
                            name = group.Key.BasicDocumentName,
                            series = group.Key.Series ?? "",
                            fromNumber = $"{range.from:D6}",
                            toNumber = $"{range.to:D6}",
                            count = range.count.ToString(),
                        });
                    }
                }
                else
                {
                    // No ranges found, create at least one model with empty range info
                    results.Add(new DocManagementApplicationReportDocumentModel
                    {
                        BasicDocumentId = group.Key.BasicDocumentId,
                        nomenclatureNumber = group.Key.BasicDocumentName.Split(" ", StringSplitOptions.RemoveEmptyEntries).FirstOrDefault() ?? "",
                        name = group.Key.BasicDocumentName,
                        series = group.Key.Series ?? "",
                        fromNumber = "",
                        toNumber = "",
                        count = "",
                    });
                }
            }

            // If no series groups found, create at least one model
            if (!results.Any())
            {
                results.Add(new DocManagementApplicationReportDocumentModel
                {
                    BasicDocumentId = doc.BasicDocumentId,
                    nomenclatureNumber = doc.BasicDocumentName.Split(" ", StringSplitOptions.RemoveEmptyEntries).FirstOrDefault() ?? "",
                    name = doc.BasicDocumentName,
                    series = "",
                    fromNumber = "",
                    toNumber = "",
                    count = "",
                });
            }

            return results;
        }

        private static string GetDefaultSeriesForDocument(DocManagementApplicationBasicDocumentModel doc, DocManagementApplicationDeliveredDocModel item)
        {
            if (string.IsNullOrEmpty(doc.SeriesFormat))
            {
                return null;
            }

            // Parse SeriesFormat patterns like "Г-YY|ГА-YY" and take the first option
            var seriesOptions = doc.SeriesFormat.Split('|');
            var firstSeriesOption = seriesOptions.FirstOrDefault();

            if (string.IsNullOrEmpty(firstSeriesOption))
            {
                return null;
            }

            // For duplicate documents, handle special logic
            if (doc.IsDuplicate)
            {
                if (item.Edition.HasValue)
                {
                    // For duplicates with editions, create format "SeriesFormat - Edition"
                    return $"{firstSeriesOption}/{item.Edition.Value}";
                }
                else
                {
                    // If no edition available, use just the series format
                    return firstSeriesOption;
                }
            }
            else
            {
                // For non-duplicate documents, handle YY patterns
                if (firstSeriesOption.Contains("YY"))
                {
                    if (item.Edition.HasValue)
                    {
                        return firstSeriesOption.Replace("YY", (item.Edition.Value % 100).ToString("D2"));
                    }
                    else
                    {
                        return firstSeriesOption.Replace("YY", (doc.SchoolYear + 1).ToString("D2"));
                    }
                }
                else
                {
                    // Series without year pattern
                    return firstSeriesOption;
                }
            }
        }

        private static string GetSeriesForDocument(DocManagementApplicationBasicDocumentModel doc, DocManagementApplicationDeliveredDocModel item)
        {
            // If item already has a series, use it
            if (!string.IsNullOrEmpty(item.Series))
            {
                return item.Series;
            }

            // Generate series from SeriesFormat
            return GetDefaultSeriesForDocument(doc, item);
        }

        private static List<(int from, int to, int count)> CreateNumberRanges(IEnumerable<string> docNumbers)
        {
            var ranges = new List<(int from, int to, int count)>();
            var numbers = docNumbers
                .Where(docNum => int.TryParse(docNum, out _))
                .Select(int.Parse)
                .OrderBy(x => x)
                .ToList();

            if (!numbers.Any())
            {
                return ranges;
            }

            int rangeStart = numbers[0];
            int rangeEnd = numbers[0];

            for (int i = 1; i < numbers.Count; i++)
            {
                if (numbers[i] == rangeEnd + 1)
                {
                    rangeEnd = numbers[i];
                }
                else
                {
                    int count = rangeEnd - rangeStart + 1;
                    ranges.Add((rangeStart, rangeEnd, count));
                    rangeStart = numbers[i];
                    rangeEnd = numbers[i];
                }
            }
            
            // Add the last range
            int finalCount = rangeEnd - rangeStart + 1;
            ranges.Add((rangeStart, rangeEnd, finalCount));

            return ranges;
        }


        public static IEnumerable<DocManagementApplicationReportDocumentModel> ProcessDocumentGroups(List<DocManagementApplicationBasicDocumentModel> basicDocuments)
        {
            var docGroups = basicDocuments
                .GroupBy(x => x.BasicDocumentId)
                .Select(g => new DocManagementApplicationBasicDocumentModel
                {
                    BasicDocumentId = g.Key,
                    BasicDocumentName = g.First().BasicDocumentName,
                    DeliveredItems = g.SelectMany(x => x.DeliveredItems).ToList()
                });

            return docGroups
                .SelectMany(doc => ProcessSingleDocument(doc))
                .OrderBy(x => x.nomenclatureNumber)
                .ThenBy(x => x.series);
        }
    }
}

using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using PetaPoco.Utilities;

namespace PetaPoco.Tests.Unit.Utilities
{
    public partial class PagingHelperTests
    {
        private static string Normalize(string sql) => Regex.Replace(sql.Trim(), @"\s+", " ");

        private static SQLParts Split(string sql)
        {
            PagingHelper.Instance.SplitSQL(sql, out var parts);
            return parts;
        }

        private (TimeSpan Duration, SQLParts Parts) SplitWithDuration(string sql)
        {
            var sw = Stopwatch.StartNew();

            var parts = Split(sql);
            sw.Stop();
            var duration = sw.Elapsed;

            return (duration, parts);
        }

        private const string ComplexSqlTemplate = @"SELECT {0}
    OuterQuery.SalesYear,
    OuterQuery.SubcategoryName,
    OuterQuery.ProductLine,
    OuterQuery.TotalSubcategoryRevenue,
    (
        SELECT MAX({0} InnerPrice.UnitPrice)
        FROM Sales.SalesOrderDetail AS InnerPrice
        WHERE InnerPrice.SalesOrderID IN (
            SELECT TOP 100 SubID.SalesOrderID 
            FROM Sales.SalesOrderHeader AS SubID 
            ORDER BY SubID.TotalDue DESC
        )
    ) AS MaxPriceThreshold
FROM (
    SELECT 
        DerivedSales.SalesYear,
        DerivedSales.SalesMonth,
        ps.Name AS SubcategoryName,
        DerivedSales.ProductLine,
        SUM(DerivedSales.GrossRevenue) AS TotalSubcategoryRevenue,
        MAX(DerivedSales.MaxLineItemRevenue) AS PeakLineItemRevenue
    FROM (
        -- Deeply nested query with its own GROUP BY
        SELECT 
            DATEPART(year, o.OrderDate) AS SalesYear,
            DATEPART(month, o.OrderDate) AS SalesMonth,
            p.ProductSubcategoryID,
            p.ProductLine,
            SUM(od.UnitPrice * od.OrderQty * (1 - od.UnitPriceDiscount)) AS GrossRevenue,
            MAX({0} od.UnitPrice * od.OrderQty) AS MaxLineItemRevenue
        FROM Sales.SalesOrderHeader AS o
        INNER JOIN Sales.SalesOrderDetail AS od ON o.SalesOrderID = od.SalesOrderID
        INNER JOIN Production.Product AS p ON od.ProductID = p.ProductID
        WHERE o.Status = 5
          AND o.OrderDate >= '2025-01-01'
        GROUP BY 
            DATEPART(year, o.OrderDate),
            DATEPART(month, o.OrderDate),
            p.ProductSubcategoryID,
            p.ProductLine
    ) AS DerivedSales
    LEFT OUTER JOIN Production.ProductSubcategory AS ps ON DerivedSales.ProductSubcategoryID = ps.ProductSubcategoryID
    WHERE DerivedSales.ProductLine IN ('M', 'R', 'T')
    GROUP BY 
        DerivedSales.SalesYear,
        DerivedSales.SalesMonth,
        ps.Name,
        DerivedSales.ProductLine
) AS OuterQuery
INNER JOIN (
    -- Subquery utilizing TOP + ORDER BY to bypass standard subquery order restrictions
    SELECT TOP 100 PERCENT
        p_inner.ProductSubcategoryID,
        AVG(od_inner.UnitPrice * od_inner.OrderQty) AS MonthlyAverage,
        MAX(od_inner.OrderQty) AS MaxQtyOrdered
    FROM Sales.SalesOrderDetail od_inner
    JOIN Production.Product p_inner ON od_inner.ProductID = p_inner.ProductID
    GROUP BY p_inner.ProductSubcategoryID
    ORDER BY MonthlyAverage DESC, MaxQtyOrdered ASC
) AS SubAgg ON OuterQuery.TotalSubcategoryRevenue > SubAgg.MonthlyAverage
WHERE OuterQuery.PeakLineItemRevenue < (SELECT MAX(UnitPrice) FROM Sales.SalesOrderDetail)";

        private const string GroupByClause = @"
GROUP BY 
    OuterQuery.SalesYear,
    OuterQuery.SubcategoryName,
    OuterQuery.ProductLine,
    OuterQuery.TotalSubcategoryRevenue";

        private const string OrderByClause = @"
ORDER BY 
    OuterQuery.SalesYear ASC, 
    OuterQuery.TotalSubcategoryRevenue DESC";
    }
}

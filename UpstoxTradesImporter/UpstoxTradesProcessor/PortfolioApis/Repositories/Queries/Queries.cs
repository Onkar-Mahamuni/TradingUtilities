namespace PortfolioApis.Repositories
{
    public static class Queries
    {
        public static readonly string CagrQuery = @"USE Trading;
                                WITH CTE_Trades AS (
                                    SELECT 
                                        Company,
                                        Side,
                                        Quantity,
                                        Price,
                                        Date,
                                        ROW_NUMBER() OVER(PARTITION BY Company, Side ORDER BY Date) AS rn
                                    FROM Trades
                                ),
                                CTE_Buy AS (
                                    SELECT 
                                        Company,
                                        SUM(Quantity * Price) AS TotalBuy,
                                        COUNT(*) AS BuyDays,
                                        SUM(Quantity * Price) / SUM(Quantity) AS AvgBuyPrice, -- Corrected average buying price
                                        DATEADD(DAY, AVG(DATEDIFF(DAY, 0, Date)), 0) AS AvgBuyDate,
                                        SUM(Quantity) AS TotalBuyQuantity
                                    FROM CTE_Trades
                                    WHERE Side = 'Buy'
                                    GROUP BY Company
                                ),
                                CTE_Sell AS (
                                    SELECT 
                                        Company,
                                        SUM(Quantity * Price) AS TotalSell,
                                        COUNT(*) AS SellDays,
                                        AVG(Price) AS AvgSellPrice,
                                        DATEADD(DAY, AVG(DATEDIFF(DAY, 0, Date)), 0) AS AvgSellDate,
                                        SUM(Quantity) AS TotalSellQuantity
                                    FROM CTE_Trades
                                    WHERE Side = 'Sell'
                                    GROUP BY Company
                                ),
                                CTE_ClosedPositions AS (
                                    SELECT 
                                        B.Company,
                                        POWER(S.TotalSell / (B.TotalBuy * S.TotalSellQuantity / B.TotalBuyQuantity), 1.0 / ((B.BuyDays + S.SellDays) / 2.0)) - 1 AS CAGR,
                                        B.AvgBuyPrice,
                                        S.AvgSellPrice,
                                        DATEDIFF(DAY, B.AvgBuyDate, S.AvgSellDate) AS HoldingPeriod,
                                        (S.TotalSell - B.TotalBuy * S.TotalSellQuantity / B.TotalBuyQuantity) AS AbsoluteProfit,
                                        ((S.TotalSell - B.TotalBuy * S.TotalSellQuantity / B.TotalBuyQuantity) / (B.TotalBuy * S.TotalSellQuantity / B.TotalBuyQuantity)) * 100 AS PercentageProfit,
                                        S.TotalSellQuantity AS Quantity,
                                        (S.TotalSell - B.TotalBuy * S.TotalSellQuantity / B.TotalBuyQuantity) / S.TotalSellQuantity AS ProfitPerShare
                                    FROM CTE_Buy B
                                    JOIN CTE_Sell S ON B.Company = S.Company
                                    WHERE B.TotalBuyQuantity >= S.TotalSellQuantity
                                ),
                                CTE_OpenPositions AS (
                                    SELECT 
                                        B.Company,
                                        NULL AS CAGR,
                                        B.AvgBuyPrice,
                                        NULL AS AvgSellPrice,
                                        NULL AS HoldingPeriod,
                                        NULL AS AbsoluteProfit,
                                        NULL AS PercentageProfit,
                                        B.TotalBuyQuantity - ISNULL(S.TotalSellQuantity, 0) AS Quantity,
                                        NULL AS ProfitPerShare
                                    FROM CTE_Buy B
                                    LEFT JOIN CTE_Sell S ON B.Company = S.Company
                                    WHERE B.TotalBuyQuantity > ISNULL(S.TotalSellQuantity, 0)
                                )
                                SELECT 
                                    'Closed Positions' AS PositionType,
                                    Company,
                                    AvgBuyPrice,
                                    AvgSellPrice,
                                    Quantity,
                                    HoldingPeriod,
                                    ProfitPerShare,
                                    AbsoluteProfit,
                                    PercentageProfit,
                                    CAGR * 100 AS IndividualCAGR -- Corrected CAGR to percentage
                                FROM CTE_ClosedPositions
                                UNION ALL
                                SELECT 
                                    'Open Positions' AS PositionType,
                                    Company,
                                    AvgBuyPrice,
                                    AvgSellPrice,
                                    Quantity,
                                    HoldingPeriod,
                                    ProfitPerShare,
                                    AbsoluteProfit,
                                    PercentageProfit,
                                    CAGR * 100 AS IndividualCAGR -- Corrected CAGR to percentage
                                FROM CTE_OpenPositions";
    }
}

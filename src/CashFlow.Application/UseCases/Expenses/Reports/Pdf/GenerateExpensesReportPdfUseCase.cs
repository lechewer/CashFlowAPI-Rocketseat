using CashFlow.Application.UseCases.Expenses.Reports.Pdf.Colors;
using CashFlow.Application.UseCases.Expenses.Reports.Pdf.Fonts;
using CashFlow.Domain.Extensions;
using CashFlow.Domain.Reports;
using CashFlow.Domain.Repositories.Expenses;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Tables;
using MigraDoc.Rendering;
using PdfSharp.Fonts;

namespace CashFlow.Application.UseCases.Expenses.Reports.Pdf;
public class GenerateExpensesReportPdfUseCase : IGenerateExpensesReportPdfUseCase
{
    private readonly string CURRENCY_SYMBOL = ResourceReportGenerationMessage.CURRENCY_SYMBOL;
    private const int HEIGHT_ROW_EXPENSE_TABLE = 25;
    private readonly IExpensesReadOnlyRespository _repository;

    public GenerateExpensesReportPdfUseCase(IExpensesReadOnlyRespository repository)
    {
        _repository = repository;

        GlobalFontSettings.FontResolver = new ExpensesReportFontResolver();
    }

    public async Task<byte[]> Execute(DateOnly month)
    {
        var expenses = await _repository.FilterByMonth(month);
        if (expenses.Count == 0)
        {
            return [];
        }

        var document = CreateDocument(month);
        var page = CreatePage(document);

        CreateHeader(page);
        
        var totalExpenses = expenses.Sum(expense => CurrencyConverter(expense.Amount));
        CreateTotalSpentSection(page, month, totalExpenses);

        foreach (var expense in expenses)
        {
            var table = CreateExpenseTable(page);
            
            var row = table.AddRow();
            row.Height = HEIGHT_ROW_EXPENSE_TABLE;
            
            AddExpenseTitle(row.Cells[0], expense.Title);
            AddHeaderForAmount(row.Cells[3]);

            row = table.AddRow();
            row.Height = HEIGHT_ROW_EXPENSE_TABLE;
            
            row.Cells[0].AddParagraph(expense.Date.ToString("D"));
            SetStyleBaseForExpenseInformation(row.Cells[0]);
            row.Cells[0].Format.LeftIndent = 20;
            
            row.Cells[1].AddParagraph(expense.Date.ToString("t"));
            SetStyleBaseForExpenseInformation(row.Cells[1]);
            
            row.Cells[2].AddParagraph(expense.PaymentMethod.PaymentTypeToString());
            SetStyleBaseForExpenseInformation(row.Cells[2]);

            AddAmountForExpense(row.Cells[3], CurrencyConverter(expense.Amount));

            if (!expense.Description.IsValueNullOrEmpty())
            {
                AddDescriptionForExpense(table, expense.Description!);
                
                row.Cells[3].MergeDown = 1;
            }
            table.AddRow().Height = 30;
        }
        return RenderDocument(document);
    }
 
    private void AddDescriptionForExpense(Table table, string description)
    {
        var row = table.AddRow();
        row.Height = HEIGHT_ROW_EXPENSE_TABLE;
        
        row.Cells[0].AddParagraph(description);
        row.Cells[0].Format.Font = new Font { Name = FontHelper.WORKSANS_REGULAR, Size = 10, Color = ColorsHelper.BLACK };
        row.Cells[0].Shading.Color = ColorsHelper.GREEN_LIGHT;
        row.Cells[0].VerticalAlignment = VerticalAlignment.Center;
        row.Cells[0].Format.Alignment = ParagraphAlignment.Left;
        row.Cells[0].Format.LeftIndent = 20;
        row.Cells[0].MergeRight = 2;
    }
    private void AddAmountForExpense(Cell cell, decimal amount)
    {
        cell.AddParagraph($"- {CURRENCY_SYMBOL}{amount}");
        cell.Format.Font = new Font { Name = FontHelper.WORKSANS_REGULAR, Size = 12, Color = ColorsHelper.BLACK };
        cell.Shading.Color = ColorsHelper.WHITE;
        cell.VerticalAlignment = VerticalAlignment.Center;
        cell.Format.Alignment = ParagraphAlignment.Right;


    }
    private void SetStyleBaseForExpenseInformation(Cell cell)
    {
        cell.Format.Font = new Font { Name = FontHelper.WORKSANS_REGULAR, Size = 12, Color = ColorsHelper.BLACK };
        cell.Shading.Color = ColorsHelper.GREEN_DARK;
        cell.VerticalAlignment = VerticalAlignment.Center;
    }
    private void AddHeaderForAmount(Cell cell)
    {
        cell.AddParagraph(ResourceReportGenerationMessage.AMOUNT);
        cell.Format.Font = new Font { Name = FontHelper.RALEWAY_BLACK, Size = 14, Color = ColorsHelper.WHITE };
        cell.Shading.Color = ColorsHelper.RED_DARK;
        cell.VerticalAlignment = VerticalAlignment.Center;
        cell.Format.Alignment = ParagraphAlignment.Right;
    }
    private void AddExpenseTitle(Cell cell, string expenseTitle)
    {
        cell.AddParagraph(expenseTitle);
        cell.Format.Font = new Font { Name = FontHelper.RALEWAY_BLACK, Size = 14, Color = ColorsHelper.BLACK };
        cell.Shading.Color = ColorsHelper.RED_LIGHT;
        cell.VerticalAlignment = VerticalAlignment.Center;
        cell.MergeRight = 2;
        cell.Format.LeftIndent = 20;
    }
    private decimal CurrencyConverter(decimal expense)
    {
        var moneyType = ResourceReportGenerationMessage.CURRENCY_SYMBOL;

        var moneyValue = moneyType switch
        {
            "R$" => 5.46M,
            "€" => 0.92M,
            _ => 1M
        };

        var amount = expense * moneyValue;
        amount = Convert.ToDecimal(amount.ToString("N2"));
        return amount;
    }
    private Table CreateExpenseTable(Section page)
    {
        var table = page.AddTable();
        
        table.AddColumn("195").Format.Alignment = ParagraphAlignment.Left;
        table.AddColumn("80").Format.Alignment = ParagraphAlignment.Center;
        table.AddColumn("120").Format.Alignment = ParagraphAlignment.Center;
        table.AddColumn("120").Format.Alignment = ParagraphAlignment.Right;
        
        return table;
    }
    private void CreateTotalSpentSection(Section page, DateOnly month, decimal totalExpenses)
    {
        var paragraph = page.AddParagraph();
        paragraph.Format.SpaceBefore = 40;
        paragraph.Format.SpaceAfter = 40;
        var title = string.Format(ResourceReportGenerationMessage.TOTAL_SPENT_IN, month.ToString("Y"));

        paragraph.AddFormattedText(title, new Font { Name = FontHelper.RALEWAY_REGULAR, Size = 15 });

        paragraph.AddLineBreak();
        
        
        paragraph.AddFormattedText($"{CURRENCY_SYMBOL} {totalExpenses}", new Font { Name = FontHelper.WORKSANS_BLACK, Size = 50 });

    }
    private Document CreateDocument(DateOnly month)
    {
        var document = new Document();

        document.Info.Title = $"{ResourceReportGenerationMessage.EXPENSES_FOR} {month:Y}";
        document.Info.Author = "Welisson Arley";

        var style = document.Styles["Normal"];
        style!.Font.Name = FontHelper.RALEWAY_REGULAR;

        return document;
    }
    private void CreateHeader(Section page)
    {
        
        var table = page.AddTable();
        
        // table.AddColumn();
        table.AddColumn("300");

        var row = table.AddRow();
        
        /*
         * var assembly = Assembly.GetExecutingAssembly();
         * var directoryName = Path.GetDirectoryName(assembly.Location);
         * row.Cells[0].AddImage(Path.Combine(directoryName, *nomeDaPastaDaFoto*, *nomeDaFoto*);
         * row.Cells[0].AddImage("/Users/igor/Downloads/photo_test.png");
         */
      
        row.Cells[0].AddParagraph("Hey, Igor Martins");
        row.Cells[0].Format.Font = new Font { Name = FontHelper.RALEWAY_BLACK, Size = 16 };
        row.Cells[0].VerticalAlignment = VerticalAlignment.Center;
        row.Cells[0].Format.Alignment = ParagraphAlignment.Left;  

    }
    private Section CreatePage(Document document)
    {
        var section = document.AddSection();
        section.PageSetup = document.DefaultPageSetup.Clone();
        
        section.PageSetup.PageFormat = PageFormat.A4;

        section.PageSetup.LeftMargin = 40;
        section.PageSetup.RightMargin = 40;
        section.PageSetup.TopMargin = 80;
        section.PageSetup.BottomMargin = 80;

        return section;
    }
    private byte[] RenderDocument(Document document)
    {
        var renderer = new PdfDocumentRenderer
        {
            Document = document,
        };

        renderer.RenderDocument();

        using var file = new MemoryStream();
        renderer.PdfDocument.Save(file);

        return file.ToArray();
    }
}

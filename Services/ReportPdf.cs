using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

public class ReportService
{
    private readonly ApplicationContext _context;

    public ReportService(ApplicationContext context)
    {
        _context = context;
    }

    public byte[] GenerateUserSalesReport(Guid userId)
    {
        var sales = _context.Sales
            .Where(s => s.UserId == userId)
            .ToList();

        using (var memoryStream = new MemoryStream())
        {
            var document = new Document();
            PdfWriter.GetInstance(document, memoryStream);
            document.Open();

            var existingUser = _context.Users.Find(userId);


            document.Add(new Paragraph($"Sales Report for User: {existingUser.Name}\nID: {userId}"));
            document.Add(new Paragraph(" "));

            var table = new PdfPTable(5);  // Количество столбцов в таблице
            table.AddCell("Sale ID");
            table.AddCell("Name");
            table.AddCell("Date");
            table.AddCell("Amount");
            table.AddCell("Price");

            foreach (var sale in sales)
            {
                table.AddCell(sale.SaleId.ToString());
                table.AddCell(sale.Name);
                table.AddCell(sale.Date.ToString("d"));
                table.AddCell(sale.Amount.ToString());
                table.AddCell(sale.Price.ToString("C"));
            }

            document.Add(table);
            document.Close();

            return memoryStream.ToArray();
        }
    }
}

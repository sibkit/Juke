using System;
using System.ComponentModel.DataAnnotations;

namespace Flux.Pcb.Data;

public class PcbOrder 
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [Required, MaxLength(100)]
    public string CustomerName { get; set; } = string.Empty;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public PcbOrderStatus Status { get; set; } = PcbOrderStatus.Uploading;
    
    // --- Данные, извлеченные BoardFlow ---
    public double WidthMm { get; set; }
    public double LengthMm { get; set; }
    public int LayersCount { get; set; }
    
    // --- Пользовательские настройки ---
    [MaxLength(20)]
    public string SolderMaskColor { get; set; } = "Green";
    
    [MaxLength(20)]
    public string SilkscreenColor { get; set; } = "White";
    
    public double ThicknessMm { get; set; } = 1.6;
    public int Quantity { get; set; } = 5;
    
    // --- Финансы и файлы ---
    public decimal CalculatedPrice { get; set; }
    
    [MaxLength(500)]
    public string ZipFilePath { get; set; } = string.Empty;
}

public enum PcbOrderStatus 
{
    Uploading,       // Архив загружается
    Analyzing,       // BoardFlow парсит гербера
    Draft,           // Пользователь настраивает параметры
    AwaitingPayment, // Ожидает оплаты
    Paid,            // Оплачено, в работу
    InProduction,    // На заводе
    Shipped          // Отправлен клиенту
}
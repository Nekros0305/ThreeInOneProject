using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicTacToe.Data.Settings
{
    public class MauiSettings : IDataBaseSettings
    {
        string IDataBaseSettings.PathToFile { get; set; } = string.Empty;
        string IDataBaseSettings.DBFileName { get => "GameRecords.db"; set { } }
    }
}

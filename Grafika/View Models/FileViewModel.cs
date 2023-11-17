using Grafika.Commands;
using Grafika.Enums;
using Grafika.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace Grafika.View_Models
{
    public interface IFileViewModel : INotifyPropertyChanged
    {
        public ICommand LoadImage { get; set; }
        public ICommand SaveImage { get; set; }
        public ICommand ConvertedImage { get; set; }
    }
    public class FileViewModel: IFileViewModel
    {
        private readonly IFileSerivce _fileService;
        private FileWindow fileWindow;
        public FileViewModel(IFileSerivce fileServic,FileWindow _fileWindow)
        {
           fileWindow= _fileWindow;
            _fileService = fileServic;
            LoadImage = new RelayCommand(Load);
            SaveImage = new RelayCommand(Save);
            ConvertedImage = new RelayCommand(Image);
        }
        public ICommand LoadImage { get; set; }
        public ICommand SaveImage { get; set; }
        public ICommand ConvertedImage { get; set; }

        private void Save(object sender)
        {
            
        }
        private void Load(object sender)
        {
            _fileService.LoadImage(fileWindow);
        }
        private void Image(object sender)
        {

        }
        public event PropertyChangedEventHandler? PropertyChanged;
    }
}

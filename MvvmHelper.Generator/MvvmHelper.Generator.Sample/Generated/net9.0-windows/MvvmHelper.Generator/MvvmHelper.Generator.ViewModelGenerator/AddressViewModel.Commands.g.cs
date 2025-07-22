using System;
using System.Windows.Input;
using MvvmHelper.Commands;
namespace MvvmHelper.Generator.Sample;

public partial class AddressViewModel
{
        private DelegateCommand _LoadedCommand = null;
    public ICommand LoadedCommand
    {
       get
       {
           if(_LoadedCommand is null)
           {
               _LoadedCommand = new DelegateCommand(Loaded);
           }
           return _LoadedCommand;
       }
    }
}
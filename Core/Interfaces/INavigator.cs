using KeyReassigner.UI;

namespace KeyReassigner.Interfaces
{
    interface INavigator
    {
        void NavigateTo(WindowTypes windowType);
        void Exit();
    }
}
using PowerCloud.ViewModels;

namespace PowerCloud
{
    public interface IPdfRender
    {
        void Render(string acessToken, MainNasFileViewModel fileSelected);
    }
}

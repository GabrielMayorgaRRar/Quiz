using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Quiz.ViewModels;

namespace Quiz.Selectors;

public class RespuestaTemplateSelector : IDataTemplate
{
    public IDataTemplate? TextoTemplate { get; set; }
    public IDataTemplate? ImagenTemplate { get; set; }
    public IDataTemplate? AudioTemplate { get; set; }

    public Control? Build(object? param)
    {
        if (param is RespuestaItem item)
        {
            return item.Tipo switch 
            {
                TipoRespuesta.Texto => TextoTemplate!.Build(param),
                TipoRespuesta.Imagen => ImagenTemplate!.Build(param),
                TipoRespuesta.Audio => AudioTemplate!.Build(param),
                _ => TextoTemplate!.Build(param)
            };
        }

        return new TextBlock { Text = "Error" };
    }

    public bool Match(object? data)
    {
        return data is RespuestaItem;
    }
}
using Microsoft.JSInterop;

namespace AlaWiki.Module.MindMap.Client;

public class Interop
{
    private readonly IJSRuntime _jsRuntime;

    public Interop(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public ValueTask<string> GetElementPosition(string elementId)
    {
        return _jsRuntime.InvokeAsync<string>("AlaWikiMindMap.getElementPosition", elementId);
    }

    public ValueTask SetElementPosition(string elementId, double x, double y)
    {
        return _jsRuntime.InvokeVoidAsync("AlaWikiMindMap.setElementPosition", elementId, x, y);
    }

    public ValueTask InitializePanZoom(string containerId)
    {
        return _jsRuntime.InvokeVoidAsync("AlaWikiMindMap.initializePanZoom", containerId);
    }

    public ValueTask ExportToSvg(string containerId)
    {
        return _jsRuntime.InvokeVoidAsync("AlaWikiMindMap.exportToSvg", containerId);
    }
}

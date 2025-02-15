﻿namespace FastWiki.Web.Rcl.Pages.Wikis;

public partial class WikiDetailInfo
{
    [Parameter] public long Id { get; set; }

    private int page = 1;

    private int pageSize = 20;

    private PaginatedListBase<WikiDetailVectorQuantityDto> _wikiDetails = new();

    private async Task Loading()
    {
        _wikiDetails = await WikiService.GetWikiDetailVectorQuantityAsync(Id.ToString(), page, pageSize);

        _ = InvokeAsync(StateHasChanged);
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await Loading();
        }
    }

    private async Task OnPageChanged(int page)
    {
        if (this.page == page)
            return;

        this.page = page;

        await Loading();
    }
}
using CentralStation.Client.Models;
using CentralStation.Data;
using CentralStation.Server.MappingExtensions;
using CentralStation.Server.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Web.Resource;

namespace CentralStation.Server.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
[RequiredScope(RequiredScopesConfigurationKey = "AzureAdB2c:Scopes")]
public class HubController : ControllerBase
{
    private readonly MainDbContext _mainDb;
    private readonly IPeerService _peerService;

    public HubController(MainDbContext mainDb, IPeerService peerService)
    {
        _mainDb = mainDb;
        _peerService = peerService;
    }

    [HttpGet("peers")]
    public IAsyncEnumerable<PeerRow> GetPeers()
    {
        return _mainDb.Peers.Include(p => p.PeerNodes).AsNoTracking().OrderBy(p => p.Name).Select(p => p.MapToPeerRow()).AsAsyncEnumerable();
    }

    // NOTE: this is not needed right away so this is not fully implemented yet (the frontend doesn't call it)
    // TODO: test and implement
    [HttpDelete("peers/{id}")]
    public async Task<IActionResult> DeletePeer(Guid id) => await _peerService.DeletePeer(id) ? Ok() : NotFound();

    [HttpGet("nodes/{type}")]
    public IAsyncEnumerable<PeerNodeRow> GetNodes(string type) => _peerService.GetNodes(type);

    [HttpDelete("nodes/{id}")]
    public async Task<IActionResult> DeleteNode(Guid id) => await _peerService.DeleteNode(id) ? Ok() : NotFound();

    [HttpGet("connections/{type}")]
    public IAsyncEnumerable<PeerConnectionRow> GetConnections(string type) => _peerService.GetConnections(type);

    [HttpDelete("connections/{id}")]
    public async Task<IActionResult> DeleteConnection(Guid id) => await _peerService.DeleteConnection(id) ? Ok() : NotFound();

    [HttpDelete("connections/expired")]
    public Task DeleteExpiredConnections() => _peerService.DeleteExpired();

    [HttpPost("connections/delete-all")]
    public Task DeleteAllConnectionsExcept([FromBody] List<string> excludedConnections) => _peerService.DeleteAllExcept(excludedConnections);
}

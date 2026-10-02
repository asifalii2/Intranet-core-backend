using AutoMapper;
using Intranet.Core.Constants;
using Intranet.Core.DTOs.Attachments;
using Intranet.Core.Interfaces.Services;
using Intranet.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace Intranet.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class AttachmentsController : ControllerBase
{
    private readonly IAttachmentService _attachmentService;
    private readonly INotificationService _notificationService;
    private readonly IAuthUser _authUser;
    private readonly IMapper _mapper;

    public AttachmentsController(
        IAttachmentService attachmentService,
        INotificationService notificationService,
        IAuthUser authUser,
        IMapper mapper)
    {
        _attachmentService = attachmentService;
        _notificationService = notificationService;
        _authUser = authUser;
        _mapper = mapper;
    }

    /// <summary>
    /// Get paginated list of attachments with filtering
    /// </summary>
    /// <param name="filter">Filter and pagination parameters</param>
    /// <returns>Paginated list of attachments</returns>
    [HttpGet]
    public async Task<ActionResult<List<AttachmentForListing>>> GetAttachmentsAsync(
        [FromQuery] AttachmentsRequestFilter filter)
    {
        var attachments = await _attachmentService.GetAttachmentsAsync(filter);

        Response.AddPaginationHeader(attachments);

        var listing = _mapper.Map<List<AttachmentForListing>>(attachments);

        return listing;
    }

    /// <summary>
    /// Upload an attachment file
    /// </summary>
    /// <param name="file">The file to upload</param>
    /// <param name="folderName">The folder name where the file should be stored (required)</param>
    /// <returns>Attachment ID</returns>
    [HttpPost]
    [RequestSizeLimit(104857600)] // 100 MB limit
    public async Task<ActionResult<AttachmentDto>> AddAttachmentAsync([FromForm] AttachmentForCreation attachment)
    {
        if (attachment.File == null || attachment.File.Length == 0)
            return BadRequest(new { Message = "File is required." });

        if (string.IsNullOrWhiteSpace(attachment.FolderName))
            return BadRequest(new { Message = "Folder name is required." });

        if (string.IsNullOrEmpty(attachment.Visibility))
            return BadRequest(new { Message = "Visibility is required." });

        var result = await _attachmentService.AddAttachmentAsync(attachment);

        #region Activity
        #endregion

        #region Notification
        #endregion

        return result;
    }


    [HttpPut]
    public async Task<ActionResult> UpdateAttachmentAsync(AttachmentForUpdate attachmentForUpdate)
    {
        var exists = await _attachmentService.IsAttachmentExistAsync(attachmentForUpdate.Id);
        if (!exists)
            return NotFound(new { Message = "Attachment Not Found." });

        if (string.IsNullOrEmpty(attachmentForUpdate.Visibility))
            return BadRequest(new { Message = "Visibility is required." });

        var updated = await _attachmentService.UpdateAttachmentAsync(attachmentForUpdate);
        if (!updated)
            return BadRequest(new { Message = "Error In Attachment Update." });

        return Ok(new { Message = "Updated successfully."});
    }

    /// <summary>
    /// Delete an attachment and remove the underlying file from S3
    /// </summary>
    /// <param name="attachmentId">The ID of the attachment to delete</param>
    [HttpDelete("{attachmentId}")]
    public async Task<ActionResult> DeleteAttachmentAsync([FromRoute] int attachmentId)
    {
        var exists = await _attachmentService.IsAttachmentExistAsync(attachmentId);

        if (!exists)
            return NotFound(new { Message = "Attachment Not Found." });

        // Get attachment info before deletion for notification
        var attachment = await _attachmentService.GetAttachmentByIdAsync(attachmentId);
        var fileName = attachment?.FileName ?? "an attachment";

        var deleted = await _attachmentService.DeleteAttachmentAsync(attachmentId);

        if (!deleted)
            return BadRequest(new { Message = "Error In Attachment Deletion." });

        #region Activity
        #endregion

        #region Notification
        #endregion

        return Ok(new { Message = "Deleted Successfully." });
    }

    /// <summary>
    /// Get a pre-signed URL for an attachment (5 minutes expiry)
    /// </summary>
    /// <param name="attachmentId">The ID of the attachment</param>
    /// <returns>Pre-signed URL</returns>
    [HttpGet("{attachmentId}/presigned-url")]
    public async Task<ActionResult<PreSignedUrlDto>> GetPreSignedUrlAsync([FromRoute] int attachmentId)
    {
        try
        {
            var result = await _attachmentService.GetPreSignedUrlAsync(attachmentId, expirationMinutes: 5);
            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { Message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = "Error generating pre-signed URL.", Error = ex.Message });
        }
    }

    /// <summary>
    /// Download an attachment file
    /// </summary>
    /// <param name="attachmentId">The ID of the attachment to download</param>
    /// <returns>File stream</returns>
    [HttpGet("{attachmentId}/download")]
    public async Task<IActionResult> DownloadAttachmentAsync([FromRoute] int attachmentId)
    {
        try
        {
            var result = await _attachmentService.DownloadAttachmentAsync(attachmentId);
            return File(result.Stream, result.ContentType, result.FileName);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { Message = ex.Message });
        }
        catch (FileNotFoundException ex)
        {
            return NotFound(new { Message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = "Error downloading attachment.", Error = ex.Message });
        }
    }
}

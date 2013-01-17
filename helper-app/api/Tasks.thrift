namespace csharp Snaphappi.API
namespace php snaphappi_api

/**
 * The major version number for the current revision of the Snaphappi protocol.
 */
const i16 SNAPHAPPI_VERSION_MAJOR = 0

/**
 * The minor version number for the current revision of the Snaphappi protocol.
 */
const i16 SNAPHAPPI_VERSION_MINOR = 1

/**
 * A 32-bit image hash. Robust against resampling and re-compression.
 */
typedef i32 ImageHash

/**
 * Unique image identifier
 */
typedef i32 ImageID

/**
 * ID used to make sure the web page and the app stay in sync with each other.
 * At most one app instance could be running for any given ID.
 */
struct TaskID
{
	/**
	 * An authentication token for the user.
	 */
	1: required string AuthToken;

	/**
	 * A session ID, which should be reset when the user restarts a given task.
	 */
	2: optional string Session;

	/**
	 * An ID unique to every device.
	 */
	3: required string DeviceID;
}

/**
 * Information about a file to upload.
 */
struct UploadTarget
{
	/**
	 * Absolute file path.
	 */
	1: required string FilePath;

	/**
	 * File creation Unix timestamp.
	 */
	2: required i32 ExifDateTime;

	/**
	 * Unique image identifier.
	 */
	3: required ImageID ImageID;
}

/**
 SystemException type.
 */
enum ErrorCode
{
	Unknown      = 1;
	InvalidAuth  = 2;
	DataConflict = 3;
}

/**
 Exception that could be thrown by any method.
 */
exception SystemException
{
	/**
	 * The numeric code indicating the type of error that has occured.
	 */
	1: required ErrorCode ErrorCode;

	/**
	 * Optional additional information about the error.
	 * Note: it is important to avoid conflicts with native exception
	 *       fields when naming this member.
	 */
	2: optional string Information;
}

/**
 * Flags indicating the state of the task.
 */
struct URTaskState
{
	/**
	 * To be set at when the task is completed or cancelled.
	 */
	1: optional bool IsCancelled;

	/**
	 * A strictly increasing change counter for the folder list in a given task.
	 */
	2: optional i32  FolderUpdateCount;

	/**
	 * A strictly increasing change counter for the file list in a given task.
	 * Updated for files uploaded and files to upload.
	 */
	3: optional i32  FileUpdateCount;
}

/**
 * The type of file uploaded with UploadFile().
 */
enum UploadType
{
	Preview  = 1;
	Original = 2;
}

/**
 * Information about the file uploaded with UploadFile().
 */
struct UploadInfo
{
	/**
	 * The type of file being uploaded.
	 */
	1: required UploadType UploadType;

	/**
	 * Used for uploading originals, where the original file could have
	 * been moved, or renamed.
	 */
	2: optional ImageID imageID;
}

/**
 * Service for working with the task of servicing the initial upload of files
 * from the user's computer to the server.
 */
service Task
{
	/**
	 * Add a folder to search.
	 */
	void AddFolder
		( 1: TaskID id
		, 2: string path
		) throws (1: SystemException systemException);

	/**
	 * Remove a folder from search.
	 */
	void RemoveFolder
		( 1: TaskID id
		, 2: string path
		) throws (1: SystemException systemException);
		
	/**
	 * Returns the device ID associated with this session or empty string
	 * if it is not yet known.
	 */
	string GetDeviceID
		( 1: string authToken
		, 2: string sessionID
		) throws (1: SystemException systemException);

	/**
	 * Return the number of files to be uploaded from a folder.
	 */
	i32 GetFileCount
		( 1: TaskID id
		, 2: string folder
		) throws (1: SystemException systemException);

	/**
	 * Return the list of all files uploaded from the given folder within
	 * the device corresponding to the given task ID.
	 */
	list<string> GetFiles
		( 1: TaskID id
		, 2: string folder
		) throws (1: SystemException systemException);

	/**
	 * Return the list of all files to be uploaded to the server.
	 * Used to upload file originals.
	 */
	list<UploadTarget> GetFilesToUpload
		( 1: TaskID id
		) throws (1: SystemException systemException);

	/**
	 * Return the list of folders to scan for images.
	 */
	list<string> GetFolders
		( 1: TaskID id
		) throws (1: SystemException systemException);

	/**
	 * Retrieves the hash of an image.
	 */
	ImageHash GetImageHash
		( 1: TaskID  id
		, 2: ImageID imageID
		) throws (1: SystemException systemException);

	/**
	 * Retrieves flags indicating the state of the task.
	 */
	URTaskState GetState
		( 1: TaskID id
		) throws (1: SystemException systemException);

	/**
	 * Retrieves the list of folders this user has set to be watched.
	 */
	list<string> GetWatchedFolders
		( 1: TaskID id
		) throws (1: SystemException systemException);

	/**
	 * Report the number of files to be uploaded from a folder.
	 */
	void ReportFileCount
		( 1: TaskID id
		, 2: string folder
		, 3: i32    count
		) throws (1: SystemException systemException);

	/**
	 * Report that a file to be uploaded was not found.
	 */
	void ReportFileNotFound
		( 1: TaskID  id
		, 2: string  folder
		, 3: string  path
		) throws (1: SystemException systemException);

	/**
	 * Report that a file to be uploaded was not found.
	 */
	void ReportFileNotFoundByID
		( 1: TaskID  id
		, 2: ImageID imageID
		) throws (1: SystemException systemException);

	/**
	 * Report that a folder could not be searched.
	 */
	void ReportFolderNotFound
		( 1: TaskID id
		, 2: string folder
		) throws (1: SystemException systemException);

	/**
	 * Report that all files in a folder have been uploaded.
	 */
	void ReportFolderUploadComplete
		( 1: TaskID id
		, 2: string folder
		) throws (1: SystemException systemException);

	/**
	 * Report a failed upload.
	 */
	void ReportUploadFailed
		( 1: TaskID id
		, 2: string folder
		, 3: string path
		) throws (1: SystemException systemException);

	/**
	 * Report a failed upload.
	 */
	void ReportUploadFailedByID
		( 1: TaskID  id
		, 2: ImageID imageID
		) throws (1: SystemException systemException);

	/**
	 * Upload file contents, along with its path and the folder in which it
	 * was found.
	 */
	void UploadFile
		( 1: TaskID     id
		, 2: string     path
		, 3: binary     data
		, 4: UploadInfo info
		) throws (1: SystemException systemException);
}

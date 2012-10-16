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
	2: required string Session;
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
	 */
	3: optional i32  FileUpdateCount;
}

/**
 * Service for working with the task of servicing the initial upload of files
 * from the user's computer to the server.
 */
service Task
{
	/**
	 * Return the list of folders to scan for images.
	 */
	list<string> GetFolders(1: TaskID id);

	/**
	 * Return the list of all files uploaded from the given folder within
	 * the given task.
	 */
	list<string> GetFiles(1: TaskID id, 2: string folder);

	/**
	 * Report that a folder could not be searched.
	 */
	void ReportFolderNotFound(1: TaskID id, 2: string folder);

	/**
	 * Report a failed upload.
	 */
	void ReportUploadFailed(1: TaskID id, 2: string folder, 3: string path);

	/**
	 * Report that all files in a folder have been uploaded.
	 */
	void ReportFolderUploadComplete(1: TaskID id, 2: string folder);

	/**
	 * Report the number of files to be uploaded from a folder.
	 */
	void ReportFileCount(1: TaskID id, 2: string folder, 3: i32 count);

	/**
	 * Return the number of files to be uploaded from a folder.
	 */
	i32 GetFileCount(1: TaskID id, 2: string folder);

	/**
	 * Retrieves flags indicating the state of the task.
	 */
	URTaskState GetState(1: TaskID id);

	/**
	 * Add a folder to search.
	 */
	void AddFolder
		( 1: TaskID id
		, 2: string path
		);
	/**
	 * Upload file contents, along with its path and the folder in which it
	 * was found.
	 */
	void UploadFile
		( 1: TaskID id
		, 2: string path
		, 3: binary data
		);
}

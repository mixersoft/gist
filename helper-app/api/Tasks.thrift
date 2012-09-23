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
 */
struct TaskID
{
	1: required i32    Task;
	2: required string Session;
}

/**
 * Flags indicating the state of the task.
 */
struct URTaskState
{
	1: optional bool IsCancelled;
}

/**
 * Service for working with the task of servicing the initial upload of files
 * from the user's computer to the server.
 */
service URTaskControl
{
	/**
	 * Return the list of folders to scan for images.
	 */
	list<string> GetFolders(1: TaskID id);
}

service URTaskInfo
{
	/**
	 * Retrieves flags indicating the state of the task.
	 */
	URTaskState GetState(1: TaskID id);
}

service URTaskUpload
{
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

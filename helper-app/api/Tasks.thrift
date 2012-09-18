namespace csharp Snaphappi.Tasks
namespace php snaphappi_tasks

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
typedef string SessionID

/**
 * Service for working with the task of servicing the initial upload of files
 * from the user's computer to the server.
 */
service InitialUploadTask
{
	 list<string> GetFolders(1: SessionID sessionID)
}

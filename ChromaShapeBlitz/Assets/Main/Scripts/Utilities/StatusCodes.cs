
public readonly struct StatusCodes
{
    public const int BEGIN_WRITE_BINARY_SERIALIZE = 100;
    public const int DONE_WRITE_BINARY_SERIALIZE  = 101;
    public const int BEGIN_BUILD_LEVEL_MENU       = 102;
    public const int DONE_BUILD_LEVEL_MENU        = 103;

    public const int BEGIN_READ_BINARY_SERIALIZE = 200;
    public const int DONE_READ_BINARY_SERIALIZE = 201;

    public const int BEGIN_WRITE_USER_DATA = 10;
    public const int BEGIN_READ_USER_DATA = 20;
    public const int DONE_WRITE_USER_DATA = 11;
    public const int DONE_READ_USER_DATA = 21;
}
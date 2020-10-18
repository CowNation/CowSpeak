## <div align="center">Functions</div>
Type | Name | Parameter 0 | Parameter 1 | Parameter 2
--- | --- | --- | --- | ---
decimal | Abs | decimal value
void | Any.Delete | 
object | Any.GetField | string fieldName
object | Any.GetIndexer | ObjectArray index
object | Any.GetProperty | string propertyName
object | Any.InvokeMethod | string methodName | ObjectArray parameters
void | Any.SetField | string fieldName | object value
void | Any.SetProperty | string propertyName | object value
ByteArray | Any.ToByteArray | 
string | Any.ToString | 
object | Array.Get | integer index
integer | Array.Length | 
void | Array.Set | integer index | object value
boolean | boolean.Flip | 
object | BooleanArray.Create | integer length | ObjectArray values
character | byte.ToCharacter | 
string | byte.ToHexadecimal | 
integer | byte.ToInteger | 
object | ByteArray.Create | integer length | ObjectArray values
string | ByteArray.GetString | string encoding
integer | character.ToInteger | 
character | character.ToLower | 
character | character.ToUpper | 
object | CharacterArray.Create | integer length | ObjectArray values
void | ClearConsole | 
decimal | Cos | decimal num
object | DecimalArray.Create | integer length | ObjectArray values
void | Define | string from | string to
void | DeleteFile | string filePath
boolean | DoesFileExist | string filePath
object | EvaluateExpression | string expression
void | ExecuteFile | string filePath
void | Exit | integer exitCode
object | FromByteArray | string typeName | ByteArray bytes
string | GetCurrentFile | 
integer | GetCurrentLine | 
integer | GetCurrentMilliseconds | 
integer | GetCurrentSeconds | 
boolean | GetDebug | 
string | GetHtmlFromUrl | string url
character | InputCharacter | 
character | InputKey | 
string | InputString | 
character | integer.ToCharacter | 
string | integer.ToHexadecimal | 
character | integer64.ToCharacter | 
string | integer64.ToHexadecimal | 
object | IntegerArray.Create | integer length | ObjectArray values
object | InvokeStaticMethod | string typeName | string methodName | ObjectArray parameters
object | ObjectArray.Create | integer length | ObjectArray values
decimal | Pow | decimal x | decimal y
void | Print | object text
integer | RandomInteger | integer minimum | integer maximum
StringArray | ReadFileLines | string filePath
decimal | Round | decimal value
void | Run | string filePath
decimal | Sin | decimal num
void | Sleep | integer ms
decimal | Sqrt | decimal value
character | string.CharacterAt | integer index
ByteArray | string.GetBytes | string encoding
integer | string.IndexOf | string value
string | string.Insert | integer index | string value
integer | string.LastIndexOf | string value
integer | string.Length | 
integer | string.OccurrencesOf | string counter
string | string.Remove | integer index | integer length
string | string.Replace | string oldValue | string newValue
string | string.SubString | integer index | integer length
decimal | string.ToDecimal | 
integer | string.ToInteger | 
object | StringArray.Create | integer length | ObjectArray values
decimal | Tan | decimal num
void | ThrowError | string errorText
void | WriteFileLines | string filePath | StringArray lines


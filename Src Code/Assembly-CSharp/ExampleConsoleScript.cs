using System;
using UnityEngine;

// Token: 0x02000034 RID: 52
public class ExampleConsoleScript : MonoBehaviour
{
	// Token: 0x06000117 RID: 279 RVA: 0x0000AEA5 File Offset: 0x000090A5
	[ConsoleMethod("echo", "Repeats what you pass to it.")]
	private void echoString(string s)
	{
		Debug.Log(s);
	}

	// Token: 0x06000118 RID: 280 RVA: 0x0000AEAD File Offset: 0x000090AD
	[ConsoleMethod]
	private void unnamedCommand()
	{
		Debug.Log("unnamedCommand was executed");
	}

	// Token: 0x06000119 RID: 281 RVA: 0x0000AEB9 File Offset: 0x000090B9
	[ConsoleMethod("static")]
	private static void staticCommand()
	{
		Debug.Log("static was executed");
	}

	// Token: 0x0600011A RID: 282 RVA: 0x0000AEC5 File Offset: 0x000090C5
	[ConsoleMethod("test", "This command just logs some stuff.")]
	private void testHelp()
	{
		Debug.Log("This is a normal log");
		Debug.LogWarning("This is a warning log");
		Debug.LogError("This is a error log");
	}

	// Token: 0x0600011B RID: 283 RVA: 0x0000AEE5 File Offset: 0x000090E5
	[ConsoleMethod("", "This method had no name given")]
	private void noName()
	{
		Debug.Log("noname was executed!");
	}

	// Token: 0x0600011C RID: 284 RVA: 0x0000AEF4 File Offset: 0x000090F4
	[ConsoleMethod("colour", "Changes the colour of given object ID. Usage: objectID, red, green, blue")]
	[ConsoleMethod("color", "Changes the colour of given object ID. Usage: objectID, red, green, blue")]
	private void ChangeColour(GameObject go, byte red, byte green, byte blue)
	{
		if (go == null)
		{
			Debug.LogError("Cannot change colour of a null object");
			return;
		}
		Debug.Log("Changing " + go.name + " colour");
		Color32 c = new Color32(red, green, blue, byte.MaxValue);
		go.GetComponent<Renderer>().material.color = c;
	}

	// Token: 0x0600011D RID: 285 RVA: 0x0000AF58 File Offset: 0x00009158
	[ConsoleMethod("details", "Logs the transform details of a object")]
	private void LogTransformDetails(GameObject go)
	{
		if (go == null)
		{
			Debug.LogError("Cannot get details of a null object");
			return;
		}
		Debug.Log("Details of " + go.name + ": " + "\nPosition: " + go.transform.position + "\nRotation: " + go.transform.rotation.eulerAngles + "\nLocal Scale: " + go.transform.localScale);
	}

	// Token: 0x0600011E RID: 286 RVA: 0x0000AFEC File Offset: 0x000091EC
	[ConsoleMethod("move", "Moves the supplied gameobject to the position. Example Usage: move 7380 (0, 10, 0)")]
	private void Move(GameObject go, Vector3 position)
	{
		if (go == null)
		{
			Debug.LogError("Cannot move a null object");
			return;
		}
		go.transform.position = position;
		Debug.Log(string.Concat(new object[]
		{
			"Moved ",
			go.name,
			" to ",
			go.transform.position
		}));
	}
}

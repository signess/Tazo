using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Threading.Tasks;
using System;

public class DialogManager : MonoBehaviour
{
    public static DialogManager Instance { get; private set; }

    [SerializeField] private GameObject dialogBox;
    [SerializeField] private TextMeshProUGUI dialogText;
    [SerializeField] private int lettersPerSecond;

    private Dialog dialog;
    private Action onDialogFinished;

    private int currentLine = 0;
    private bool isTyping;

    public event Action OnShowDialog;
    public event Action OnCloseDialog;

    public bool IsShowing { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    public void HandleUpdate()
    {
        if(Input.GetKeyDown(KeyCode.Z) && !isTyping)
        {
            ++currentLine;
            if(currentLine < dialog.Lines.Count)
            {
                TypeDialog(dialog.Lines[currentLine]).GetAwaiter();

            }
            else
            {
                currentLine = 0;
                IsShowing = false;
                dialogBox.SetActive(false);
                onDialogFinished?.Invoke();
                OnCloseDialog?.Invoke();
            }
        }
    }

    public IEnumerator ShowDialog(string text, bool waitForInput = true)
    {
        IsShowing = true;
        dialogBox.SetActive(true);

        yield return new WaitUntil(()=> TypeDialog(text).IsCompleted);
        if(waitForInput)
        {
            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Z));
        }

        dialogBox.SetActive(false);
        IsShowing = false;
    }

    public async Task ShowDialog(Dialog dialog, Action onFinished = null)
    {
        await Task.Yield();

        OnShowDialog?.Invoke();
        IsShowing = true;
        this.dialog = dialog;
        onDialogFinished = onFinished;
        dialogBox.SetActive(true);
        await TypeDialog(dialog.Lines[0]);

    }

    public async Task TypeDialog(string dialog)
    {
        isTyping = true;
        dialogText.text = "";
        foreach(var letter in dialog.ToCharArray())
        {
            dialogText.text += letter;
            await Task.Delay(1000 / lettersPerSecond);
        }
        isTyping = false;
    }
}

using UnityEngine;
using UnityEngine.UI;

public class FragmentsController : MonoBehaviour
{
    [Header("Base settings")]
    [SerializeField] private Panel panel;
    [SerializeField] private Color fragmentColor;
    [SerializeField] private Color takenFragmentColor;
    [SerializeField] private Image[] fragments;
    [SerializeField] private Hints hints;

    [HideInInspector] private int fragmentsCount;
    [HideInInspector] private int totalFragmentsCount;

    private void UpdateColors()
    {
        for (var index = 0; index < totalFragmentsCount; index++)
        {
            Image image = fragments[index];
            image.color = takenFragmentColor;
        }

        for (var index = 0; index < fragmentsCount; index++)
        {
            Image image = fragments[index];
            image.color = fragmentColor;
        }
    }

    public bool HasUnusedFragment()
    {
        return fragmentsCount > 0;
    }

    [ContextMenu("AddFragment")]
    public void AddFragment()
    {
        if (totalFragmentsCount < 4)
        {
            fragmentsCount++;
            totalFragmentsCount++;
            hints.ShowHint(0);

            UpdateColors();
        }
    }

    [ContextMenu("RemoveFragment")]
    public void RemoveFragment()
    {
        if (HasUnusedFragment())
        {
            fragmentsCount--;
            UpdateColors();
            
            if (totalFragmentsCount >= 4 && fragmentsCount <= 0)
            {
                panel.Win();
            }
            hints.ShowHint(1, new object[] {totalFragmentsCount - fragmentsCount, fragments.Length});
        }
    }
}

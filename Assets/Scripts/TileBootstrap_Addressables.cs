using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.Tilemaps;

public class TileBootstrap_Addressables : MonoBehaviour
{
    [Header("Addressables Label")]
    [SerializeField] private string label = "Tile";

    private Loading _loadingSystem;

    public AsyncOperationHandle<IList<Tile>> LoadHandle;
    private bool _loaded;

    private void Awake()
    {
        _loadingSystem = FindAnyObjectByType<Loading>();
    }

    private void Start()
    {
       
        // Optionnel: si tu veux être certain que le Registry existe
        if (TileRegistry.Instance == null)
            Debug.LogError("TileRegistry manquant dans la scène (ajoute-le sur un GameObject bootstrap).");

        Load();
    }

    public void Load()
    {
        if (_loaded) return;

        LoadHandle = Addressables.LoadAssetsAsync<Tile>(
            label,
            callback: null // ou (sprite) => SpriteRegistry.Instance.Register(sprite)
        );

        LoadHandle.Completed += handle =>
        {
            if (handle.Status != AsyncOperationStatus.Succeeded)
            {
                Debug.LogError($"Addressables: échec chargement label '{label}'");
                return;
            }

            // Enregistrer dans le registry
            TileRegistry.Instance.RegisterMany(handle.Result);

            _loaded = true;
            Debug.Log($"Addressables: chargé {handle.Result.Count} tiles (label '{label}')");
            Destroy(this);
        };
    }

    private void OnDestroy()
    {
        // Si cet objet vit toute la durée du jeu (DontDestroyOnLoad), tu peux release seulement à la fin.
        // Si tu changes de “packs” (par niveau), c’est ici que tu fais le release.
        if (_loaded && LoadHandle.IsValid())
        {
            Addressables.Release(LoadHandle);
            _loaded = false;
        }
    }
}
using UnityEngine;

public class Chest : Interactable
{
    [Header("Настройки сундука")]
    public WeaponData weaponInside; // Перетащите сюда меч из иерархии
    public GameObject chestLid;
    private bool isOpened = false;

    public override void BaseInteract()
    {
        if (isOpened) return;
        isOpened = true;

        promptMessage = "Сундук открыт";

        // Выдаем оружие игроку
        GiveWeaponToPlayer();

        if (chestLid != null)
        {
            chestLid.transform.Rotate(-90f, 0f, 0f);
        }
    }

    private void GiveWeaponToPlayer()
    {
        if (weaponInside == null)
        {
            Debug.LogError("В сундуке не назначено оружие!");
            return;
        }

        PlayerCombat playerCombat = FindObjectOfType<PlayerCombat>();
        if (playerCombat != null)
        {
            // Вызываем метод у игрока, передавая ему наш меч
            playerCombat.EquipWeapon(weaponInside);
        }
    }
}
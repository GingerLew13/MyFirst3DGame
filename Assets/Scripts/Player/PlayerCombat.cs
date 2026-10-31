using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCombat : MonoBehaviour
{
    [Header("Ссылки")]
    public Camera playerCamera;
    public Animator animator;

    [Header("Текущее оружие")]
    public WeaponData equippedWeapon;
    public Transform weaponHolder; // Пустой объект в руке игрока (или у камеры)

    private void Update()
    {
        // Проверка нажатия ЛКМ через новый Input System
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            TryAttack();
        }
    }

    private void TryAttack()
    {
        // Если оружия нет, или камера не назначена - ничего не делаем
        if (equippedWeapon == null || playerCamera == null) return;

        // Проверка кулдауна атаки
        if (Time.time < equippedWeapon.nextAttackTime) return;

        // Запускаем анимацию (если есть)
        if (animator != null)
        {
            animator.SetTrigger("Attack");
        }

        if (AudioManager.Instance != null) AudioManager.Instance.PlaySwordSwing();

        // Устанавливаем время следующей атаки
        equippedWeapon.nextAttackTime = Time.time + (1f / equippedWeapon.attackRate);

        // Выполняем логику удара
        PerformHit();
    }

    private void PerformHit()
    {
        // Луч вылетает из центра камеры (куда смотрит игрок)
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);

        // Рисуем луч в редакторе для отладки (видно только в Scene View)
        Debug.DrawRay(ray.origin, ray.direction * equippedWeapon.attackRange, Color.red, 1f);

        // Если попали во что-то в пределах дистанции оружия и нужного слоя
        if (Physics.Raycast(ray, out RaycastHit hit, equippedWeapon.attackRange, equippedWeapon.enemyLayer))
        {
            EnemyAI enemy = hit.collider.GetComponent<EnemyAI>();
            if (enemy != null)
            {
                enemy.TakeDamage(equippedWeapon.damage);

                // ЗВУК ПОПАДАНИЯ
                if (AudioManager.Instance != null) AudioManager.Instance.PlayEnemyHit();
            }
        }
    }

    // --- НОВЫЙ МЕТОД ЭКИПИРОВКИ ---
    public void EquipWeapon(WeaponData newWeapon)
    {
        if (newWeapon == null)
        {
            Debug.LogError("Попытка экипировать пустое оружие (null)!");
            return;
        }

        // 1. Если в руках было старое оружие, полностью его отключаем
        if (equippedWeapon != null && equippedWeapon != newWeapon)
        {
            equippedWeapon.gameObject.SetActive(false);
        }

        // 2. Берем новое оружие
        equippedWeapon = newWeapon;

        // 3. Включаем 3D-модель нового оружия
        equippedWeapon.gameObject.SetActive(true);

        // 4. Прикрепляем к руке игрока (если слот назначен)
        if (weaponHolder != null)
        {
            // Делаем меч дочерним объектом руки
            equippedWeapon.transform.SetParent(weaponHolder);

            // Выравниваем позицию и поворот, чтобы он идеально лег в руку
            equippedWeapon.transform.localPosition = Vector3.zero;
            equippedWeapon.transform.localRotation = Quaternion.identity;

            // Если модель меча слишком большая или маленькая, раскомментируйте и настройте:
            // equippedWeapon.transform.localScale = Vector3.one; 
        }
        else
        {
            Debug.LogWarning("WeaponHolder не назначен в инспекторе! Оружие не прикрепится к руке.");
        }
    }
}
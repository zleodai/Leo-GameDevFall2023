using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ItemInterface
{
    void interact();
    void interactcancel();
    void equip();
    void unequip();
    void pickup();
    void drop();
}

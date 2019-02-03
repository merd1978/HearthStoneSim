using HearthStoneSimCore.Enums;
using HearthStoneSimCore.Model;

namespace HearthStoneSimCore.Actions
{
    public static partial class GameAction
    {
        public static bool AttackBlock(Controller player, Character source, Character target)
        {
            //if (!PreAttackPhase.Invoke(c, source, target))
            //    return false;
            //if (!OnAttackTrigger.Invoke(c, source, target))
            //{
            //    // end block
            //    if (c.Game.History)
            //        c.Game.PowerHistory.Add(PowerHistoryBuilder.BlockEnd());
            //    return false;
            //}
            if (!AttackPhase(player, source, target))
            {
                // end block
                //if (c.Game.History)
                //    c.Game.PowerHistory.Add(PowerHistoryBuilder.BlockEnd());
                return false;
            }
            // end block
            //if (c.Game.History)
            //    c.Game.PowerHistory.Add(PowerHistoryBuilder.BlockEnd());
            player.Game.DeathProcessingAndAuraUpdate();
            player.Game.StateChanged = true;
            return true;
        }

        public static bool AttackPhase(Controller player, Character source, Character target)
        {
            //game.TaskQueue.StartEvent();
            //game.TriggerManager.OnTargetTrigger(source);
            //game.ProcessTasks();
            //game.TaskQueue.EndEvent();

            var hero = source as Hero;
            var minion = source as Minion;

            //var target = (ICharacter)game.CurrentEventData.EventTarget;

            // Force the game into MAIN_COMBAT step!
            //Game.Step = Step.MAIN_COMBAT;

            // Save defender's attack as it might change after being damaged (e.g. enrage)
            var targetHero = target as Hero;
            int targetAttack = /*targetHero != null ? 0 : */target.AttackDamage;
            int sourceAttack = /*hero?.TotalAttackDamage ?? */source.AttackDamage;

            int targetRealDamage = target.TakeDamage(source, sourceAttack);
            bool targetDamaged = targetRealDamage > 0;

            // freeze target if attacker is freezer
            if (targetDamaged && minion != null && minion.Freeze)
            {
                player.Game.Log(LogLevel.VERBOSE, BlockType.ATTACK, "AttackPhase", "freezer attacker has frozen target.");
                target.IsFrozen = true;
            }

            // destroy target if attacker is poisonous
            if (targetDamaged && targetHero == null && (minion != null && minion.Poisonous || hero?.Weapon != null && hero.Weapon.Poisonous) && !target.ToBeDestroyed)
            {
                player.Game.Log(LogLevel.VERBOSE, BlockType.ATTACK, "AttackPhase", $"poisonous attacker has destroyed target.");
                target.Destroy();
            }

            // ignore damage from defenders with 0 attack
            if (targetAttack > 0)
            {
                int sourceRealDamage = source.TakeDamage(target, targetAttack);
                bool sourceDamaged = sourceRealDamage > 0;

                // freeze source if defender is freezer
                var targetMinion = target as Minion;
                if (sourceDamaged && targetMinion != null && targetMinion.Freeze)
                {
                    source.IsFrozen = true;
                }

                // destroy source if defender is poisonous
                if (sourceDamaged && targetMinion != null && targetMinion.Poisonous && !source.ToBeDestroyed)
                {
                    source.Destroy();
                }
            }

            if (minion != null && minion.HasStealth)
            {
                minion.HasStealth = false;
            }

            // remove durability from weapon if hero attack
            if (hero?.Weapon != null && !hero.Weapon.IsImmune)
            {
                //hero.Weapon.Durability -= hero.Weapon.Durability > 0 ? 1 : 0;
                hero.Weapon.Damage += 1;
            }

            source.IsAttacking = false;
            target.IsDefending = false;

            int numAtk = source.NumAttacksThisTurn + 1;

            player.NumOptionsPlayedThisTurn++;
            if (minion != null)
                player.NumFriendlyMinionsThatAttackedThisTurn++;

            // set exhausted ...
            if (numAtk > 0 && !source.HasWindfury ||
                numAtk > 1 && source.HasWindfury)
            {
                player.Game.Log(LogLevel.INFO, BlockType.ATTACK, "AttackPhase", $"{source} is now exhausted.");
                source.IsExhausted = true;
            }

            source.NumAttacksThisTurn = numAtk;

            player.Game.Log(LogLevel.INFO, BlockType.ATTACK, "AttackPhase",
                $"{source}[ATK:{source.AttackDamage}/HP:{source.Health}{(hero != null ? $"/ARM:{hero.Armor}" : "")}] " +
                $"{(hero?.Weapon != null ? $"[{hero.Weapon}[A:{hero.Weapon.AttackDamage}/D:{hero.Weapon.Durability}]] " : "")}attacked " +
                $"{target}[ATK:{target.AttackDamage}/HP:{target.Health}].");
            return true;
        }

    }
}

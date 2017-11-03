using HearthStoneSimCore.Enums;
using HearthStoneSimCore.Model;

namespace HearthStoneSimCore.Actions
{
    public static class Attack
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
            var sourceHero = source as Hero;
            var sourceMinion = source as Minion;
            //source.ProposedAttacker = source.Id;
            //var target = c.Game.IdEntityDic[source.ProposedDefender] as ICharacter;
            if (target == null)
            {
                player.Game.Log(LogLevel.INFO, BlockType.ATTACK, "AttackPhase", "target wasn't found by proposed defender call.");
                return false;
            }
            //c.Game.Step = Step.MAIN_COMBAT;

            // Save defender's attack as it might change after being damaged (e.g. enrage)
	        var targetAttack = target is Hero ? 0 : target.AttackDamage;
            var sourceAttack = sourceHero?.TotalAttackDamage ?? source.AttackDamage;

            var targetDamaged = target.TakeDamage(source, sourceAttack);

            if (targetDamaged) target.IsDamaged = true;

            // freeze target if attacker is freezer
            if (targetDamaged &&  source.Freeze)
            {
                target.IsFrozen = true;
            }

            // destroy target if attacker is poisonous
            //if (targetDamaged && (minion != null && minion.Poisonous || hero?.Weapon != null && hero.Weapon.Poisonous))
            //{
            //    target.Destroy();
            //}

            // ignore damage from defenders with 0 attack
            if (targetAttack > 0)
            {
                var sourceDamaged = source.TakeDamage(target, targetAttack);

                if (sourceDamaged) source.IsDamaged = true;

                // freeze source if defender is freezer
                if (sourceDamaged &&  target.Freeze)
                {
                    source.IsFrozen = true;
                }

                // destroy source if defender is poisonous
                //if (sourceDamaged && targetMinion != null && targetMinion.Poisonous)
                //{
                //    source.Destroy();
                //}
            }

            if (sourceMinion != null && sourceMinion.HasStealth)
            {
                sourceMinion.HasStealth = false;
            }

            // remove durability from weapon if hero attack
            //if (hero?.Weapon != null)
            //{
            //    hero.Weapon.Durability -= hero.Weapon.Durability > 0 ? 1 : 0;
            //}

            source.IsAttacking = false;
            target.IsDefending = false;

            source.NumAttacksThisTurn++;
            //c.NumOptionsPlayedThisTurn++;
            //if (minion != null)
            //    c.NumFriendlyMinionsThatAttackedThisTurn++;

            // set exhausted ...
            if (source.NumAttacksThisTurn > 0 && !source.HasWindfury ||
                source.NumAttacksThisTurn > 1 && source.HasWindfury)
            {
                player.Game.Log(LogLevel.INFO, BlockType.ATTACK, "AttackPhase", $"{source} is now exhausted.");
                source.IsExhausted = true;
            }

            player.Game.Log(LogLevel.INFO, BlockType.ATTACK, "AttackPhase",
                $"{source}[ATK:{source.AttackDamage}/HP:{source.Health}" +
                $"{(sourceHero != null ? $"/ARM:{sourceHero.Armor}" : "")}] " +
                $"{(sourceHero?.Weapon != null ? $"[{sourceHero.Weapon}[A:{sourceHero.Weapon.AttackDamage}/D:{sourceHero.Weapon.Durability}]] " : "")}attacked " +
                $"{target}[ATK:{target.AttackDamage}/HP:{target.Health}].");
            return true;
        }
    }
}

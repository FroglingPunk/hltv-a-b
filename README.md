# [obsolete] hltv-a-b

hltv.org parser for testing theory and finding potential matches for betting.
At the time of 2024.02.11, performance has not been verified due to site updates due to the transition to cs go 2.

The theory was based on the following points:
- with a certain difference in the performance of the teams, the chance of defeat in two pistol rounds on one map was quite low
- the coefficient for winning in pistol rounds is unreasonably high (in my opinion) and was on average 1.8x

Based on them, I developed 2 strategies:

Strategy 1
- doubling strategy, for example, if you lost in the first pistol round, x2 was bet on the second

A little math:

BS - Base rate
Sk - Average coefficient (~1.8)
Vpp - probability of winning a bet in the first round
VPW - probability of bet winning in the second (pistol) round
Shn - chance of failure of both bets
Shv - chance of winning one of the bets

X1 = Bs * (sk -1) - winnings when winning the first pistol round
X2 = 2 * Bs * (sk -1) - Bs - win if you win the second pistol round, if you double the bet after losing the first
X3 = X1 * Vpp + X2 * Vpp - average winning
X3 * Shw = money plus
Bs * 3 * Shn = money minus

Therefore it is necessary that

X3 * Shv > Bs * 3 * Shn
Or
(Bs * (sk -1) * Vpp + (2 * Bs * (sk-1) - Bs) * Vpv) * Shv > Bs * 3 * Shn

Let's substitute the worst values for which the first pistol round is always lost:
BS = 1
Sk = 1.8
Vpp = 0
Vdv = 1

And it turns out that

0.6 * W > 3 * W

It is necessary that the Shv be at least 83.3% (in the worst situation if the first pistol round is always lost, in the best situation the minimum drops to 79%)

Therefore, we parse and look for matches in which the analyzer, based on the collected statistics, predicts a chance above this minimum.
The analyzer collects the results of ALL matches on hltv.org and looks at what indicators of the teams and the difference between them the chance of victory was sufficient to become a plus and shows upcoming matches that are potential for betting.

But if 83% may be a deterrent, there is an even safer option (however, this will lead to a decrease in potential matches found).

Strategy 2
The second approach is to ignore the first pistol round and bet on the second if the first one is lost.

Bs * (sk-1) * Shv > Bs * Shn
0.8 * W > 1 * W

And now, to reach a plus level, the minimum Shv should be only ~56%. The difference is huge.

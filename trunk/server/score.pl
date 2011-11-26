#! /usr/bin/perl
use CGI;
$CgiObj = CGI::new();
print $CgiObj->header;

@lines = split(';', LoadScore());

my $nick=$CgiObj->param('nick');
my $score=$CgiObj->param('score');
my $hash=$CgiObj->param('hash');
#check hash
if ($score>0) {
	push @lines, "$nick=$score";
		@lines = sort {
				(split '=', $b)[1] <=> (split '=', $a)[1];
						} @lines;
							if ($#lines>49) { $#lines=49; }
								SaveScore();
								}

								print join(';', @lines);

								sub LoadScore {
									my $filename = 'score.txt';
										open(fileh, $filename) || return "ONNI=200;ONNI=150;ONNI=100";
											lockfile(fileh);
												chomp(my @tmp=<fileh>);
													unlockfile(fileh);
														close(fileh);
															return join("\r\n", @tmp);
															}

															sub SaveScore {
																my $filename = 'score.txt';
																	open(fileh, ">$filename") || return;
																		lockfile(fileh);
																			print fileh join(';', @lines);
																				unlockfile(fileh);
																					close fileh;
																					}

																					sub lockfile {
																						my $count = 0;
																							my $handle = shift;
																								until (flock($handle, 2)) {
																										sleep .10;
																												if(++$count > 50) {
																															return;
																																	}

																																		}

																																		}

																																		sub unlockfile {
																																			my $handle = shift;
																																				flock($handle, 8);
																																				} 

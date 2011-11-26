#! /usr/bin/perl
use CGI;
$CgiObj = CGI::new();
print $CgiObj->header;

@lines = split ';', LoadScore();

print '<html><head><title>Best</title><body><center><table border="1">';

for ($index = 0; $index <= $#lines; $index++) {
	my @v = split '=', @lines[$index];
	print "<tr><td>@{v[0]}</td><td>@{v[1]}</td></td>";
}
print '</table></center></body></html>';

sub LoadScore {
	my $filename = 'score.txt';
	open(fileh, $filename) || return "ONNI=200;ONNI=150;ONNI=100";
	lockfile(fileh);
	chomp(my @tmp=<fileh>);
	unlockfile(fileh);
	close(fileh);
	return join("\r\n", @tmp);
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

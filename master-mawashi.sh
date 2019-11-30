for i in {1..6}; do
  if [ $i -eq 1 ]; then
    cat master-mawashi.cs | perl -pe "s/__namei__//" | perl -pe "s/__numberi__/1/" > mawashi.cs
  else
    cat master-mawashi.cs | perl -pe "s/__(name|number)i__/${i}/" > mawashi${i}.cs
  fi
done
